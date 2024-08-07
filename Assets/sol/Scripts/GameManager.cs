using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PhotonView photonView;

    public bool enablePickupCountdown = true;
    public bool itemsRunAway = false;

    // item counts
    public int maxItems;
    public int currentItems;

    // TESTING ONLY!!
    // DO NOT USE IN FINAL BUILD (for now)
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        /* get item goal number thingy
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        maxItems = items.Length;
        */
    }

    public void PickupItem()
    {
        currentItems++;

        if (currentItems >= maxItems)
        {
            Debug.Log("GAME ENDED GOAL WOOOOHHHH YEAHH BABYYYYYYY");
        }
    }

    public void StartGame(bool RPC = false)
    {
        Debug.Log("Game Started");

        if (RPC)
        {
            photonView.RPC("RPCStartGame",RpcTarget.Others);
        }

        MenuManager.Instance.OpenMenu("empty");
        ClientManager.Instance.LoadScene("sol nainofdsi sdfg", "Lobby");
        PlayerMovement.Instance.DestroyPlayer();

        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    [PunRPC] 
    private void RPCStartGame()
    {
        Debug.Log("Remote user started game!");
        StartGame();
    }

    public void EndGame(bool RPC = false)
    {
        Debug.Log("Game Ended");

        if (RPC)
        {
            photonView.RPC("RPCEndGame", RpcTarget.Others);
        }

        MenuManager.Instance.OpenMenu("Lobby");
        ClientManager.Instance.LoadScene("Lobby", "sol nainofdsi sdfg");
        PlayerMovement.Instance.DestroyPlayer();
    }

    [PunRPC] 
    private void RPCEndGame()
    {
        Debug.Log("Remote user ended game!");
        EndGame();
    }
}
