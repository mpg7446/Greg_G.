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

    private Vector3 origin;

    // visuals - slider
    public GameObject sliderObj = null;
    private ItemSlider slider;

    // visuals - object
    private GameObject visual;
    private double bobbingOffset;
    private double bobbingMiddlePoint;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        origin = transform.position;

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

        // visual bobbing setup
        visual = transform.Find("Visuals").gameObject;
        bobbingMiddlePoint = visual.transform.localPosition.y;
        bobbingOffset = UnityEngine.Random.Range(0, 10);

        // player list setup for escaping players
        GameObject[] tempObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in tempObjects)
        {
            allPlayers.Add(player);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // check to see if interacts with player
        {

            players.Add(collision.gameObject);
            Debug.Log("player added to players list at id " + (players.Count - 1));

            if (players.Count == 1)
            {
                UpdateHolding(collision.gameObject);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // check to see if interacts with player
        {
            // remove touching value if that was the last player leaving
            if (players.IndexOf(collision.gameObject) == 0 && players.Count == 1)
            {
                touching = false;

                collision.GetComponent<PlayerInventory>().LeaveItem();
            }

            // clear player from players list
            players.Remove(collision.gameObject);
        }
    }

    // FixedUpdate checks and updates countdown values, as well as applying
    public void FixedUpdate()
    {
        // check if players are touching
        if (!touching && players.Count >= 1)
        {
            UpdateHolding(players[0]);
        }

        // update counter and check for counter completion
        if (touching)
        {
            countdown -= 2;

            if (countdown < 0)
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

    private void UpdateHolding(GameObject player)
    {
        // get player inventory script and copy countdown toggle
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        enableCountdown = inventory.enablePickupCountdown;

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
        players[0].GetComponent<PlayerInventory>().PickupItem(score);
        Destroy(gameObject);
    }

    private void RunFromPlayers()
    {
        foreach (GameObject player in allPlayers)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (!isReturning && distance < runDistance)
            {
                Vector2 angle = (transform.position - player.transform.position)/runMultiplier * distance;
                //rb.isKinematic = false;
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
}
