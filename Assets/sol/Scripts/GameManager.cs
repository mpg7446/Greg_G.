using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool enablePickupCountdown = true;
    public bool itemsRunAway = false;

    // item counts
    public int maxItems;
    public int currentItems;

    // TESTING ONLY!!
    // DO NOT USE IN FINAL BUILD
    public void Start()
    {
        // get item goal number thingy
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        maxItems = items.Length;
    }

    public void PickupItem()
    {
        currentItems++;

        if (currentItems >= maxItems)
        {
            Debug.Log("GAME ENDED GOAL WOOOOHHHH YEAHH BABYYYYYYY");
        }
    }
}
