using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private PhotonView photonView;
    public int itemCount = 0;
    private PlayerManager movement;
    private bool local = true;

    private GameObject item;

    public void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            movement = GetComponent<PlayerManager>();
        }
        else
        {
            local = false;
        }

        //ScoreCounter.Instance.AddCounter(this);
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

    public void PickupItem(int amount = 1, bool rpc = false)
    {
        itemCount += amount;
        GameManager.Instance.PickupItem();
        if (local)
        {
            movement.UpdateWeight(amount);
            photonView.RPC("PickupItem", RpcTarget.Others, amount);
        }
    }
    public void PickupItem()
    {
        PickupItem(1);
    }
    [PunRPC]
    public void PickupItem(int amount = 1)
    {
        PickupItem(amount);
    }
}
