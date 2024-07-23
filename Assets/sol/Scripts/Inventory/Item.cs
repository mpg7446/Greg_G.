using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] private int score = 1;

    public float countdown;
    private float countdownLimit = 0;
    private bool enableCountdown;

    private bool touching;
    [SerializeField] private List<GameObject> players = new List<GameObject>();

    public GameObject sliderObj = null;
    private Slider slider;

    public void Start()
    {
        countdownLimit = countdown;

        if (sliderObj == null)
        {
            sliderObj = transform.Find("Slider").gameObject;
        }

        slider = sliderObj.GetComponent<Slider>();
        slider.value = 0;
        slider.maxValue = countdownLimit;

        sliderObj.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // check to see if interacts with player
        {

            players.Add(collision.gameObject);

            if (players.Count == 1)
            {
                if (enableCountdown)
                {
                    // get player inventory script and copy countdown toggle
                    PlayerInventory inventory = collision.GetComponent<PlayerInventory>();
                    enableCountdown = inventory.enablePickupCountdown;

                    if (inventory.HoldingItem()) // if player is already holding an item, remove player from list
                    {
                        players.Remove(collision.gameObject);
                    } 
                    else // if player isnt holding an item, set touching value and add item to players holding to prevent player from picking up multiple items
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
}
