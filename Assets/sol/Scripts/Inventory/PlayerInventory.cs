using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private PhotonView photonView;
    public int itemCount = 0;
    private PlayerMovement movement;
    private bool local = true;

    private GameObject item;

    public void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            movement = GetComponent<PlayerMovement>();
        }
        else
        {
            local = false;
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
        GameManager.Instance.PickupItem();
        if (local)
        {
            movement.UpdateWeight(amount);
        }
    }
}
