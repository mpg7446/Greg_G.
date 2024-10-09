using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ClientManager;

public class PlayerManager : MonoBehaviour
{
    public int ID {  get; private set; }

    public static PlayerManager Instance = null;
    public PhotonView photonView;
    private Rigidbody2D rb;
    private InputManager input;

    // player visuals
    private PlayerModelManager playerModel;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerVisual playerVisual;

    // movement settings
    public float speed = 40;
    public float maxSpeed = 300;
    public float crouchMultiplier = 0.2f;
    private float moving = 0;

    public float groundDistance = 0.2f;
    public float wallDistance;

    public bool IsMoving
    {
        get { return input.movement.x != 0; }
        protected set { }
    }

    // action settings
    private bool holdingAction = false;

    // player stacking
    public bool inStack;
    public GameObject stackParent;
    public GameObject stackChild;

    public BoxCollider2D boxCollider;
    //private CircleCollider2D circleCollider;
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

    // DEBUG
    //public bool isGrounded;
    //public bool isAgainstWall;
    public LayerMask layerMask;

    private void Awake()
    {
        ClientManager.Instance.AddCameraTracker(gameObject);
        if (GetComponent<PlayerID>() != null )
            ID = GetComponent<PlayerID>().GetID();
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            Instance = this;
            input = GetComponent<InputManager>();
            rb = GetComponent<Rigidbody2D>();
        }
        else
        {
            //Destroy(this);
            enabled = false;
        }

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (playerModel == null)
        {
            playerModel = gameObject.GetComponent<PlayerModelManager>();
        }

        boxCollider = playerModel.stickers.GetComponent<BoxCollider2D>();
        //circleCollider = playerModel.stickers.GetComponent<CircleCollider2D>();
        //if (circleCollider != null) 
        //{ 
        //    circleCollider.enabled = false; 
        //}

        burstMax = burstTimer;
    }

    // movement - on physics update
    public void FixedUpdate()
    {
        if (ID == -1 && GetComponent<PlayerID>() != null)
            ID = GetComponent<PlayerID>().GetID();

        StandardMovement();
        UpdateVisualDirection();
        UpdateAnimations();

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

    #region Collisions
    public virtual void CollisionEnter(Collision2D collision)
    {
        // enable weight bursting
        canBurst = true;

        //if (!inStack && colliderDelay <= 0)
        //{
        //    if (stackParent == null && transform.position.y > otherObject.transform.position.y + (boxCollider.size.y / 1.1)) // collision is below player and players height
        //    {
        //        EnterStackAbove(otherObject);
        //    }
        //    else if (stackChild == null && transform.position.y < otherObject.transform.position.y - (boxCollider.size.y / 1.1)) // collision is above player and players height
        //    {
        //        EnterStackBelow(otherObject);
        //    }
        //}
    }

    public virtual void CollisionExit(Collision2D collision)
    {
        canBurst = false;
    }
    //public virtual void PassableExit(Collision2D collision)
    //{
    //    if (collision.gameObject == passableObject)
    //    {
    //        passableObject.GetComponent<Collider2D>().isTrigger = true;
    //        passableObject = null;
    //    }
    //}

    //public virtual void TriggerEnter(Collider2D collision)
    //{
    //    GameObject otherObject = collision.gameObject;

    //    // Walls
    //    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * input.movement.x, wallDistance);
    //    if (passableObject == null && otherObject.layer == LayerMask.NameToLayer("Passable") && hit.transform.position.y - (boxCollider.size.y / 1.1) <= transform.position.y)
    //    {
    //        passableObject = otherObject;
    //        passableObject.GetComponent<Collider2D>().isTrigger = false;
    //    }
    //}

    // Switch Colliders
    //protected virtual void SwitchCollider(bool enabled)
    //{
    //    if (enabled)
    //    {
    //        boxCollider.enabled = false;
    //        circleCollider.enabled = true;
    //    }
    //    else
    //    {
    //        boxCollider.enabled = true;
    //        circleCollider.enabled = false;
    //    }
    //}
    #endregion

    #region Movement & Actions
    private void StandardMovement() // for movement seperate from player stack
    {
        //SwitchCollider(false);
         //movement inputs
        if (input.movement.x != 0 && !IsAgainstWall())
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
        if (!jumped && input.movement.y > 0 && IsGrounded())
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
    //private void StackMovement() // for movement in a player stack
    //{
    //    SwitchCollider(true);
    //    rb.velocity = Vector3.up;
    //}

    private void Jump()
    {
        jumped = true;
        rb.AddForce(Vector2.up * jumpHeight * 100);
    }
    protected virtual float Crouch(bool enabled = true) // call this action squish in game i think
    {
        if (input.movement.y < 0)
        {
            if (!playerModel.Squished)
                playerModel.Squish();
            return crouchMultiplier;
        } 
        else if (playerModel.Squished)
        {
            playerModel.UnSquish();
        }
        return 1;
    }
    protected virtual void Action()
    {
        // when in player stack
        if (inStack)
        {
            LeaveStack();
        }
    }
    #endregion

    #region Position Checks
    protected bool IsGrounded()
    {
        boxCollider.enabled = false;
        // Check middle of player
        Debug.DrawRay(transform.position, Vector2.down * groundDistance, Color.yellow);
        if (rb.velocity.y <= 0 && Physics2D.Raycast(transform.position, Vector2.down, groundDistance, layerMask))
        {
            boxCollider.enabled = true;
            return true;
        }

        // Check left of player
        Vector3 side = new Vector3(boxCollider.bounds.size.x / 2, 0, 0);
        Debug.DrawRay(transform.position - side, Vector2.down * groundDistance, Color.yellow);
        if (rb.velocity.y <= 0 && Physics2D.Raycast(transform.position - side, Vector2.down, groundDistance, layerMask))
        {
            boxCollider.enabled = true;
            return true;
        }

        // Check right of player
        Debug.DrawRay(transform.position + side, Vector2.down * groundDistance, Color.yellow);
        if (rb.velocity.y <= 0 && Physics2D.Raycast(transform.position + side, Vector2.down, groundDistance, layerMask))
        {
            boxCollider.enabled = true;
            return true;
        }

        // False
        boxCollider.enabled = true;
        return false;
    }

    protected bool IsAgainstWall()
    {
        boxCollider.enabled = false;
        Debug.DrawRay(transform.position, (Vector2.right * input.movement.x) * wallDistance, Color.red);
        if (Physics2D.Raycast(transform.position, Vector2.right * input.movement.x, wallDistance, layerMask))
        {
            boxCollider.enabled = true;
            return true;
        }

        boxCollider.enabled = true;
        return false;
    }
    #endregion

    #region Weight and Burst Multipliers
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
    #endregion

    #region Player Stacking
    #region Enter Stack
    protected void EnterStackAbove(GameObject parent)
    {
        stackParent = parent;
        inStack = true;

        transform.position = parent.transform.position + new Vector3(0, parent.GetComponentInChildren<BoxCollider2D>().size.y);
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
        photonView.RPC("RPCEnterStackAbove", RpcTarget.Others, child.GetComponent<PlayerID>().GetID(), ID);
    }

    [PunRPC]
    public void RPCEnterStackAbove(int targetID, int senderID) // TODO DOESNT WORK WHY DOES THIS CALL FOR LOCAL PLAYER TOO???
    {
        Debug.Log("Recieve RPCEnterStackAbove: target: " + targetID + " object: " + ID);
        if (targetID == ID)
        {
            PlayerID[] senderIDs = FindObjectsOfType<PlayerID>();
            GameObject senderObject = null;
            foreach (PlayerID ID  in senderIDs)
            {
                if (ID.GetID() == senderID)
                {
                    senderObject = ID.gameObject;
                    Debug.Log($"Found target object! {ID}");
                }
            }
            stackParent = senderObject;
            inStack = true;

            transform.position = senderObject.transform.position + new Vector3(0, senderObject.GetComponent<BoxCollider2D>().size.y);
            Debug.Log("Entered Stack Above over RPC for " + ID);
        }
    }
    #endregion

    #region Leave Stack
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
    #endregion

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
    #endregion

    #region Visuals 
    // TODO - MOVE THIS TO PlayerModelManager!!!
    private void UpdateVisualDirection() 
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

    private void UpdateAnimations()
    {
        if (IsMoving && IsGrounded())
            playerModel.SetAnimation("Walking", true);
        else
            playerModel.SetAnimation("Walking", false);

        if (!IsGrounded())
            playerModel.SetAnimation("InAir", true);
        else
            playerModel.SetAnimation("InAir", false);
    }
    #endregion

    #region Destroy Player
    public void DestroyPlayer()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (enabled)
        {
            Debug.Log("Player Movement: closing movement script");
        }
    }
    #endregion
}
