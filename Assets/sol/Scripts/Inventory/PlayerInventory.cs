using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int itemCount = 0;
    private GameManager gameManager;
    private PlayerMovement movement;

    private GameObject item;

    public void Start()
    {
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        movement = GetComponent<PlayerMovement>();
    }

    public void IntersectItem(GameObject item)
    {
        if (!HoldingItem())
        {
            this.item = item;
        }
    }
    public void LeaveItem()
    {
        item = null;
    }
    public bool HoldingItem()
    {
        if (item != null)
        {
            return true;
        }
        return false;
    }

    public void PickupItem(int amount = 1)
    {
        itemCount += amount;
        gameManager.currentItems += amount;
        movement.UpdateWeight(amount);
    }
}
