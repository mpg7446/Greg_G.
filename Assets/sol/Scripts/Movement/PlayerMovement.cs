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
    private InputManager mv;

    // movement settings
    public float speed = 40;
    public float maxSpeed = 300;
    public float crouchMultiplier = 0.2f;
    private float moving = 0;

    // player stacking
    private bool inStack;
    public GameObject stackParent;
    public GameObject stackChild;

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
        mv = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody2D>();

        id = idCounter;
        idCounter++;

        gameObject.name += " " + id;

        burstMax = burstTimer;
    }

    // movement - on physics update
    public void FixedUpdate()
    {
        // movement inputs
        if (mv.movement[id].x != 0)
        {
            // acceleration
            moving += speed * mv.movement[id].x;
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
            if (moving < -maxSpeed/10)
            {
                moving += maxSpeed / 10;
            }
            else if (moving > maxSpeed/10)
            {
                moving -= maxSpeed / 10;
            } else
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
        if (!jumped && mv.movement[id].y > 0 && rb.velocity.y == 0)
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
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // colliding with player
        {
            // enable weight bursting
            canBurst = true;

            GameObject otherPlayer = collision.gameObject;

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

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // check if not colliding with player
        {
            canBurst = false;

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
        }
    }

    // Movement & Action Inputs
    private void StandardMovement() // for movement seperate from player stack
    {

    }
    private void Jump()
    {
        jumped = true;
        rb.AddForce(Vector2.up * jumpHeight * 100);
    }
    private float Crouch(bool enabled = true) // call this action squish in game i think
    {
        if (mv.movement[id].y < 0)
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
    }
    private void EnterStackBelow(GameObject child)
    {
        stackChild = child;
        inStack = true;
    }
    private void LeaveStack()
    {
        stackChild = null;
        stackParent = null;
        inStack = false;

        // TODO - change this so it shoots u in the players last facing direction
        rb.AddForce(new Vector3(maxSpeed * Time.fixedDeltaTime, rb.velocity.y));
    }

}
