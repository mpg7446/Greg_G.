using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerID : MonoBehaviour
{
    private int id = -1;
    private PhotonView photonView;

    public TextMeshPro text;

    [PunRPC]
    public void SetID(int id)
    {
        this.id = id;

        text.text = "Player " + id;
    }

    public int GetID()
    {
        return id;
    }

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            SetID(PhotonNetwork.LocalPlayer.ActorNumber);
            photonView.RPC("SetID",RpcTarget.Others, id); // sets ID for all other players in room
        }
        else
        {
            Debug.Log("i dont know what to do here, PlayerID hasnt got the correct id set");
            // this should probably get the ID from the host (or just another player i guess)
            photonView.RPC("AskForID", RpcTarget.Others);
        }
    }

    [PunRPC]
    public void AskForID()
    {
        if (photonView.IsMine)
        {
            SetID(PhotonNetwork.LocalPlayer.ActorNumber);
            photonView.RPC("SetID", RpcTarget.Others, id); // sets ID for all other players in room
        }
    }
}
