using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // !!!!
    // THIS IS A DEBUG PLAYER MOVEMENT DO NOT USE THIS IN THE FINAL VERSION
    // !!!!

    public static int idCounter;
    public int id;

    private Rigidbody2D rb;
    private InputManager input;

    // movement settings
    public float speed = 40;
    public float maxSpeed = 300;
    public float crouchMultiplier = 0.2f;
    private float moving = 0;

    // action settings
    private bool holdingAction = false;

    // player stacking
    private bool inStack;
    public GameObject stackParent;
    public GameObject stackChild;

    private BoxCollider2D boxCollider;
    private CircleCollider2D circleCollider;
    private int colliderDelay = 0;
    [SerializeField] private int colliderMaxDelay = 25;

    // player weight
    [SerializeField] private float weight = 100; 
    [SerializeField] private float weightMultiplier = 2;
    // player weight burst
    [SerializeField] private float burstMultiplier = 1.1f;
    [SerializeField] private int burstTimer = 5;
    private int burstMax;
    private bool bursting = false;
    private bool canBurst = false;

    // jump settings
    private bool jumped = false;
    public float jumpHeight = 4.75f;

    public void Start()
    {
        input = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody2D>();

        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.enabled = false;

        id = idCounter;
        idCounter++;

        gameObject.name += " " + id;

        burstMax = burstTimer;
    }

    // movement - on physics update
    public void FixedUpdate()
    {
        if (!inStack)
        {
            StandardMovement();
        } else
        {
            StackMovement();
        }

        if (input.action[id] && !holdingAction)
        {
            Action();
            holdingAction = true;
        } else if (!input.action[id] && holdingAction)
        {
            holdingAction = false;
        }

        if (colliderDelay > 0)
        {
            colliderDelay--;
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // colliding with player
        {
            // enable weight bursting
            canBurst = true;

            GameObject otherPlayer = collision.gameObject;

            if (!inStack && colliderDelay <= 0)
            {
                if (stackParent == null && transform.position.y > otherPlayer.transform.position.y + (GetComponent<BoxCollider2D>().size.y / 1.1)) // collision is below player and players height
                {
                    Debug.Log("Player " + id + " entered stack from above");
                    EnterStackAbove(otherPlayer);
                }
                else if (stackChild == null && transform.position.y < otherPlayer.transform.position.y - (GetComponent<BoxCollider2D>().size.y / 1.1)) // collision is above player and players height
                {
                    Debug.Log("Player " + id + " entered stack from below");
                    EnterStackBelow(otherPlayer);
                }
            }
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // check if not colliding with player
        {
            canBurst = false;
            /*
            GameObject otherPlayer = collision.gameObject;

            if (otherPlayer == stackChild)
            {
                stackChild = null;
                if (stackParent == null)
                {
                    inStack = false;
                }
            }
            else if (otherPlayer == stackParent)
            {
                stackParent = null;
                if (stackChild == null)
                {
                    inStack = false;
                }
            }
            */
        }
    }

    // Movement & Action Inputs
    private void StandardMovement() // for movement seperate from player stack
    {
        SwitchCollider(false);
        // movement inputs
        if (input.movement[id].x != 0)
        {
            // acceleration
            moving += speed * input.movement[id].x;
            if (moving > maxSpeed)
            {
                moving = maxSpeed;
            }
            else if (moving < -maxSpeed)
            {
                moving = -maxSpeed;
            }

            // weight burst
            if (burstTimer > 0)
            {
                burstTimer--;
                bursting = true;
            }
            else
            {
                bursting = false;
            }
        }
        else
        {
            // deceleration
            if (moving < -maxSpeed / 10)
            {
                moving += maxSpeed / 10;
            }
            else if (moving > maxSpeed / 10)
            {
                moving -= maxSpeed / 10;
            }
            else
            {
                moving = 0;
            }

            // cancel weight burst
            bursting = false;
            burstTimer = burstMax;
        }

        // update speed and burst mulitpliers
        float finalSpeed = moving * GetWeightMultiplier(bursting, burstMultiplier);

        // crouch
        finalSpeed *= Crouch();

        // jump
        if (!jumped && input.movement[id].y > 0 && rb.velocity.y == 0)
        {
            Jump();
        }
        else
        {
            jumped = false;
        }

        Vector2 trans = new Vector3(finalSpeed * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = trans;
    }
    private void StackMovement() // for movement in a player stack
    {
        SwitchCollider(true);
        rb.velocity = Vector3.up;
    }
    private void Jump()
    {
        jumped = true;
        rb.AddForce(Vector2.up * jumpHeight * 100);
    }
    private float Crouch(bool enabled = true) // call this action squish in game i think
    {
        if (input.movement[id].y < 0)
        {
            if (transform.localScale == new Vector3(1, 1, 1))
            {
                transform.localScale = new Vector3(1, 0.5f, 1);
                transform.position = new Vector3(transform.position.x, transform.position.y - (GetComponent<BoxCollider2D>().size.y / 4), 0);
            }

            return crouchMultiplier;
        } 
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            return 1;
        }
    }
    private void Action()
    {
        // when in player stack
        if (inStack)
        {
            LeaveStack();
        }
    }

    // Weight and Burst Multipliers
    public void UpdateWeight(int add)
    {
        weight += add * weightMultiplier;
    }
    public float GetWeightMultiplier(bool enabled, float multiplier)
    {
        if (enabled && canBurst)
        {
            return weight * multiplier / 100;
        }
        else
        {
            return weight / 100;
        }
    }

    // Player Stacking
    private void EnterStackAbove(GameObject parent)
    {
        stackParent = parent;
        inStack = true;

        transform.position = parent.transform.position + new Vector3(0, parent.GetComponent<BoxCollider2D>().size.y);
    }
    private void EnterStackBelow(GameObject child)
    {
        stackChild = child;
        inStack = true;
    }
    private void LeaveStack()
    {
        if (stackParent != null && stackChild != null) // if player is a middle player in stack
        {
            stackParent.GetComponent<PlayerMovement>().stackChild = stackChild;
            stackChild.GetComponent<PlayerMovement>().stackParent = stackParent;
        } 
        else // if player is an end player in stack
        {
            // CHANGE THIS TO A FUNCTION TAHT GETS CALLED SO THAT inStack IS CORRECT
            if (stackParent != null && stackChild == null)
            {
                stackParent.GetComponent<PlayerMovement>().LeaveStack(gameObject);
            }
            if (stackChild != null && stackParent == null)
            {
                stackChild.GetComponent<PlayerMovement>().LeaveStack(gameObject);
            }
        }

        stackParent = null;
        stackChild = null;
        inStack = false;
        colliderDelay = colliderMaxDelay;

        // TODO - change this so it shoots u in the players last facing direction and disables all stop onColliderEnter from calling EnterStack* for a certain amount of time
        // OR last movement direction
        transform.position += new Vector3(0, 0.5f, 0);
        rb.velocity = new Vector3(maxSpeed * 5 * Time.fixedDeltaTime, 0);
    }
    private void LeaveStack(GameObject player)
    {
        if (stackChild == player)
        {
            stackChild = null;
        } 
        else if (stackParent == player)
        {
            stackParent = null;
        }

        if (stackChild == null && stackParent == null)
        {
            inStack = false;
            colliderDelay = colliderMaxDelay;
        }
    }

    // Colliders
    private void SwitchCollider()
    {
        boxCollider.enabled = !boxCollider.enabled;
        circleCollider.enabled = !circleCollider.enabled;
    }
    private void SwitchCollider(bool enabled)
    {
        if (enabled)
        {
            boxCollider.enabled = false;
            circleCollider.enabled = true;
        } else
        {
            boxCollider.enabled = true;
            circleCollider.enabled = false;
        }
    }

}
