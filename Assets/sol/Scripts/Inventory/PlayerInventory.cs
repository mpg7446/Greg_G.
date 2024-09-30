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
        ScoreCounter.Instance.AddCounter(gameObject);

        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            movement = GetComponent<PlayerManager>();
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

    public void PickupItem(int amount = 1, bool rpc = false)
    {
        Debug.Log("PickupItem(amount) called");
        itemCount += amount;
        GameManager.Instance.PickupItem();
        if (local)
        {
            movement.UpdateWeight(amount);
            photonView.RPC("PickupItem", RpcTarget.Others, amount);
            ScoreCounter.Instance.IncreaseCounter(gameObject, amount);
        }
    }
    public void PickupItem()
    {
        Debug.Log("PickupItem() called");
        PickupItem(1, false);
    }
    [PunRPC]
    public void PickupItem(int amount = 1)
    {
        Debug.Log("RPC PickupItem() called");
        PickupItem(amount, false);
    }
}
