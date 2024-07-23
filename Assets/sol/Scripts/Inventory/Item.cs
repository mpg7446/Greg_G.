using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] private int score = 1;

    public bool enableCountdown;
    public float countdown;
    private float countdownLimit = 0;

    public bool touching;
    public GameObject player; // this should be used to check if anyone else is touching the item, only the first player to touch the item should pick it up

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
            player = collision.gameObject; // point item to currently waiting player

            if (enableCountdown)
            {
                // set touching score to positive to begin countdown
                touching = true;
            } else
            {
                // skip countdown sequence and immediately pickup item - TODO possibly change this to be a part of the players settings??
                PickupItem();
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // check to see if interacts with player
        {
            // set touching score to positive to begin countdown
            touching = false;
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
        player.GetComponent<PlayerInventory>().PickupItem(score);
        Destroy(gameObject);
    }
}
