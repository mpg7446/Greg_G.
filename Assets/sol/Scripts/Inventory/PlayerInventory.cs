using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private PhotonView photonView;
    public int itemCount = 0;
    private GameManager gameManager;
    private LocalMultiplayerPlayerMovement movement;

    private GameObject item;

    public void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            try
            {
                gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
            }
            catch { }

            movement = GetComponent<LocalMultiplayerPlayerMovement>();
        }
        else
        {
            enabled = false;
        }
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
