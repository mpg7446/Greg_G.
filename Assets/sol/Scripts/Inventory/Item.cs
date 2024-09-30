using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    // score
    [SerializeField] private int score = 1;

    private Vector3 origin;
    private GameManager gameManager;
    private PhotonView photonView;

    // countdown
    public float countdown;
    private float countdownLimit = 0;
    private bool enableCountdown;

    // players touching item
    private bool touching;
    [SerializeField] private List<GameObject> players = new List<GameObject>();

    // escape from players
    [SerializeField] private bool runFromPlayers;
    [SerializeField] private List<GameObject> allPlayers = new List<GameObject>();
    [SerializeField] private int runDistance;
    [SerializeField] private int runMultiplier;
    private bool isReturning;
    [SerializeField] private float returnTime;
    private Rigidbody2D rb;

    // visuals - slider
    public GameObject sliderObj = null;
    private ItemSlider slider;

    // visuals - object
    private GameObject visual;
    private double bobbingOffset;
    private double bobbingMiddlePoint;

    [Tooltip("Visual Item Type")] public foodType visualType = foodType.RANDOM;
    public enum foodType
    {
        RANDOM,
        BURGER,
        CHEESECAKE
    }

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        origin = transform.position;
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        photonView = GetComponent<PhotonView>();

        runFromPlayers = gameManager.itemsRunAway;
        enableCountdown = gameManager.enablePickupCountdown;

        // set countdown limit
        countdownLimit = countdown;

        // slider object setup
        if (sliderObj == null)
        {
            sliderObj = transform.Find("Slider").gameObject;
        }

        slider = sliderObj.GetComponent<ItemSlider>();
        slider.value = 0;
        slider.maxValue = countdownLimit;

        sliderObj.SetActive(false);

        // Setup Visuals
        visual = transform.Find("Visuals").gameObject;
        bobbingMiddlePoint = visual.transform.localPosition.y;
        bobbingOffset = UnityEngine.Random.Range(0, 10);

        UpdateVisuals(true);

        // player list setup for escaping players
        if (runFromPlayers)
        {
            GameObject[] tempObjects = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in tempObjects)
            {
                allPlayers.Add(player);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject.transform.parent.gameObject;

        if (obj.CompareTag("Player") && obj.GetComponent<MenuPlayerManager>() == null) // check to see if interacts with player
        {
            players.Add(obj);
            //Debug.Log("player added to players list at id " + (players.Count - 1));

            if (players.Count == 1)
            {
                UpdateHolding(obj);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject.transform.parent.gameObject;

        if (obj.CompareTag("Player")) // check to see if interacts with player
        {
            // remove touching value if that was the last player leaving
            if (players.IndexOf(obj) == 0 && players.Count == 1)
            {
                touching = false;

                obj.GetComponent<PlayerInventory>().LeaveItem();
            }

            // clear player from players list
            players.Remove(obj);
        }
    }

    // FixedUpdate checks and updates countdown values, as well as applying
    public void FixedUpdate()
    {
        ClearEmpty();

        // check if players are touching
        if (!touching && players.Count >= 1)
        {
            UpdateHolding(players[0]);
        }

        // update counter and check for counter completion
        if (touching)
        {
            countdown -= 2;

            if (countdown <= 0)
            {
                PickupItem();
            }

            UpdateSlider();
        } 
        else if (countdown < countdownLimit)
        {
            countdown += 1.5f;

            UpdateSlider();
        } 
        else if (countdown >= countdownLimit)
        {
            sliderObj.SetActive(false);
        }

        // visual bobbing
        float transformy = (float)(Math.Sin((Time.fixedTime + bobbingOffset) * 2) * 0.1 + bobbingMiddlePoint);
        visual.transform.localPosition = new Vector3(visual.transform.localPosition.x, transformy, visual.transform.localPosition.z);

        // escape from players
        if (runFromPlayers)
        {
            RunFromPlayers();
        }
    }

    public void ClearEmpty()
    {
        foreach (GameObject player in players)
        {
            if (player == null)
            {
                players.Remove(player);
            }
        }
    }

    private void UpdateHolding(GameObject player)
    {
        // get player inventory script and copy countdown toggle
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if (enableCountdown)
        {
            if (!inventory.HoldingItem())  // if player isnt holding an item, set touching value and add item to players holding to prevent player from picking up multiple items
            {
                touching = true;
                inventory.IntersectItem(gameObject);
            }
        }
        else
        {
            // skip countdown sequence and immediately pickup item - TODO possibly change this to be a part of the players settings??
            PickupItem();
        }
    }

    private void UpdateSlider()
    {
        sliderObj.SetActive(true);
        slider.value = countdownLimit - countdown;
    }
    
    private void PickupItem()
    {
        PlayerInventory inv = players[0].GetComponent<PlayerInventory>();
        if (inv.IsMine && GameManager.Instance.IsRunning)
        {
            inv.PickupItem(score, true);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void RunFromPlayers()
    {
        foreach (GameObject player in allPlayers)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (!isReturning && distance < runDistance)
            {
                Vector2 angle = (transform.position - player.transform.position)/runMultiplier * distance;
                rb.velocity += angle;
            } 
            else if (!isReturning && distance > runDistance)
            {
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.fixedDeltaTime * 3);
                if (distance > runDistance * 2)
                {
                    isReturning = true;
                }
            } 

            if (isReturning)
            {
                transform.position = Vector3.Lerp(transform.position, origin, Time.fixedDeltaTime * returnTime);
                if (Vector3.Distance(transform.position, origin) < 0.1f)
                {
                    isReturning = false;
                }
            }
        }
    }

    private void UpdateVisuals(bool RPCHost = false)
    {
        // Get Visuals Type
        if (RPCHost)
        {
            // Select foodType for item 
            int foodID = (int)visualType;
            if (visualType == foodType.RANDOM)
            {
                Array values = Enum.GetValues(typeof(foodType));
                foodID = UnityEngine.Random.Range(1, values.Length);
                visualType = (foodType)foodID;
            }

            photonView.RPC("UpdateVisuals", RpcTarget.Others, foodID);
        }

        // Disable visuals that are not equal to the visual type selected by Item script
        foreach (ItemType visualChild in visual.GetComponentsInChildren<ItemType>())
        {
            if (visualChild.foodType == visualType)
            {
                visualChild.gameObject.SetActive(true);
            } else
            {
                visualChild.gameObject.SetActive(false);
            }
        }
    }

    [PunRPC]
    private void UpdateVisuals(int foodID)
    {
        //Debug.Log("UpdateVisuals[RPC] recieved " +  foodID);
        visualType = (foodType)foodID;
        UpdateVisuals(false);
    }
}
