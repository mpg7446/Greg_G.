using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private PhotonView photonView;
    public bool IsMine { get { return photonView.IsMine; } }
    public int itemCount = 0;
    private PlayerManager movement;

    private GameObject item;

    public void Start()
    {
        ScoreCounter.Instance.AddCounter(gameObject);

        photonView = GetComponent<PhotonView>();
        movement = GetComponent<PlayerManager>();
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

    [PunRPC]
    public void PickupItem(int amount = 1, bool rpc = false)
    {
        //Debug.Log("PickupItem(amount) called");
        itemCount += amount;
        GameManager.Instance.PickupItem(this, amount);
        if (rpc && photonView.IsMine)
        {
            movement.UpdateWeight(amount);
            photonView.RPC("PickupItem", RpcTarget.Others, amount);
            ScoreCounter.Instance.IncreaseCounter(gameObject, amount);
        }
    }/*
    [PunRPC]
    public void PickupItem(int amount = 1)
    {
        //Debug.Log("RPC PickupItem() called");
        PickupItem(amount, false);
    }*/
}
