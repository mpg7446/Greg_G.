using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int score = 1;
    public float countdown;
    private float countdownLimit;
    public bool touching;
    public GameObject player; // this should be used to check if anyone else is touching the item, only the first player to touch the item should pick it up

    public void Start()
    {
        countdownLimit = countdown;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // check to see if interacts with player
        {
            // set touching score to positive to begin countdown
            touching = true;
            player = collision.gameObject;
            Debug.Log("player entered collider");
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // check to see if interacts with player
        {
            // set touching score to positive to begin countdown
            touching = false;
            Debug.Log("player left collider");
        }
    }

    public void FixedUpdate()
    {
        if (touching)
        {
            countdown -= 2;
            if (countdown < 0)
            {
                PickupItem();
            }
        } 
        else if (countdown < countdownLimit)
        {
            countdown += 1.5f;
        }
    }
    
    private void PickupItem()
    {
        player.GetComponent<PlayerInventory>().PickupItem(score);
        Destroy(gameObject);
    }
}
