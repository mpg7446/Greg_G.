using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance = null;
    protected PhotonView photonView;
    private Rigidbody2D rb;
    private InputManager input;
    protected PlayerID playerID;
    private SpriteRenderer spriteRenderer;

    // movement settings
    public float speed = 40;
    public float maxSpeed = 300;
    public float crouchMultiplier = 0.2f;
    private float moving = 0;

    // action settings
    private bool holdingAction = false;

    // player stacking
    public bool inStack;
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

    private void Start()
    {
        ClientManager.Instance.AddCameraTracker(gameObject);
        playerID = GetComponent<PlayerID>();
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            Instance = this;
            input = GetComponent<InputManager>();
            rb = GetComponent<Rigidbody2D>();
        }
        else
        {
            enabled = false;
        }

        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.enabled = false;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        burstMax = burstTimer;
    }

    private void OnDestroy()
    {
        if (enabled)
        {
            Debug.Log("Player Movement: closing movement script");
        }
    }

    // movement - on physics update
    public void FixedUpdate()
    {
        if (!inStack)
        {
            StandardMovement();
            UpdateVisuals();
        } else
        {
            StackMovement();
        }

        if (input.action && !holdingAction)
        {
            Action();
            holdingAction = true;
        } else if (!input.action && holdingAction)
        {
            holdingAction = false;
        }

        if (colliderDelay > 0)
        {
            colliderDelay--;
        }
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision)
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
                    EnterStackAbove(otherPlayer);
                }
                else if (stackChild == null && transform.position.y < otherPlayer.transform.position.y - (GetComponent<BoxCollider2D>().size.y / 1.1)) // collision is above player and players height
                {
                    EnterStackBelow(otherPlayer);
                }
            }
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // check if not colliding with player
        {
            canBurst = false;
        }
    }

    // Movement & Action Inputs
    private void StandardMovement() // for movement seperate from player stack
    {
        SwitchCollider(false);
        // movement inputs
        if (input.movement.x != 0)
        {
            // acceleration
            moving += speed * input.movement.x;
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
        if (!jumped && input.movement.y > 0 && rb.velocity.y == 0)
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
    protected virtual float Crouch(bool enabled = true) // call this action squish in game i think
    {
        if (input.movement.y < 0)
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
    protected virtual void Action()
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
    protected void EnterStackAbove(GameObject parent)
    {
        stackParent = parent;
        inStack = true;

        transform.position = parent.transform.position + new Vector3(0, parent.GetComponent<BoxCollider2D>().size.y);
        photonView.RPC("RPCEnterStackBelow", RpcTarget.Others, parent.name, name);
    }
    [PunRPC]
    public void RPCEnterStackBelow(string target, string sender) // TODO DOESNT WORK WHY DOES THIS CALL FOR LOCAL PLAYER TOO???
    {
        Debug.Log("Recieve RPCEnterStackBelow: target: " + target + " object: " + name);
        if (string.Equals(target, name))
        {
            stackChild = GameObject.Find(sender);
            inStack = true;

            Debug.Log("Entered Stack Below over RPC for " + name);
        }
    }
    protected void EnterStackBelow(GameObject child)
    {
        stackChild = child;
        inStack = true;
        photonView.RPC("RPCEnterStackAbove", RpcTarget.Others, child.GetComponent<PlayerID>().GetID(), playerID.GetID());
    }
    [PunRPC]
    public void RPCEnterStackAbove(int targetID, int senderID) // TODO DOESNT WORK WHY DOES THIS CALL FOR LOCAL PLAYER TOO???
    {
        Debug.Log("Recieve RPCEnterStackAbove: target: " + targetID + " object: " + playerID.GetID());
        if (targetID == playerID.GetID())
        {
            PlayerID[] senderIDs = FindObjectsOfType<PlayerID>();
            GameObject senderObject = null;
            foreach (PlayerID ID  in senderIDs)
            {
                if (ID.GetID() == senderID)
                {
                    senderObject = ID.gameObject;
                    Debug.Log($"Found target object! {playerID.GetID()}");
                }
            }
            stackParent = senderObject;
            inStack = true;

            transform.position = senderObject.transform.position + new Vector3(0, senderObject.GetComponent<BoxCollider2D>().size.y);
            Debug.Log("Entered Stack Above over RPC for " + playerID.GetID());
        }
    }

    // Leave stack
    protected void LeaveStack()
    {
        // change player stack for other players
        string target = null;
        if (stackParent != null)
        {
            target = stackParent.name;
        }
        if (stackChild != null)
        {
            target = stackChild.name;
        }
        photonView.RPC("RPCLeaveStack", RpcTarget.Others, target, name);

        // change current player stack settings
        stackParent = null;
        stackChild = null;
        inStack = false;
        colliderDelay = colliderMaxDelay;

        // TODO - change this so it shoots u in the players last facing direction and disables all stop onColliderEnter from calling EnterStack* for a certain amount of time
        // OR last movement direction
        transform.position += new Vector3(0, 0.5f, 0);
        rb.velocity = new Vector3(maxSpeed * 5 * Time.fixedDeltaTime, 0);
    }

    [PunRPC]
    protected void RPCLeaveStack(string target, string sender)
    {
        if (string.Equals(target, sender))
        {
            LeaveStack(GameObject.Find(sender));
        }
    }
    protected void LeaveStack(GameObject player) // PLEASE FOR THE LOVE OF GOD FIX THIS THIS IS A NIGHTMARE PLEASE GOD HELP 
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

    // Stack syncing
    [PunRPC]
    public string GetStackParent(string target)
    {
        if (target == name)
        {
            return stackParent.name;
        }
        return null;
    }
    [PunRPC]
    public string GetStackChild(string target)
    {
        if (target == name)
        {
            return stackChild.name;
        }
        return null;
    }

    // Colliders
    private void SwitchCollider()
    {
        boxCollider.enabled = !boxCollider.enabled;
        circleCollider.enabled = !circleCollider.enabled;
    }
    protected void SwitchCollider(bool enabled)
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

    public void DestroyPlayer()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private void UpdateVisuals()
    {
        if (input.movement.x > 0)
        {
            spriteRenderer.flipX = true;
        } 
        else if (input.movement.x < 0)
        {
            spriteRenderer.flipX = false;
        }
    }

}
