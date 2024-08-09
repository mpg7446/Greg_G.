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
    public List<GameObject> itemSpawners;

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
        } else
        {
            SpawnItems();
        }

        MenuManager.Instance.OpenMenu("empty");
        ClientManager.Instance.LoadScene("sol nainofdsi sdfg", "Lobby");
        PlayerMovement.Instance.DestroyPlayer();

        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    private void SpawnItems()
    {
        // Spawn items over network (PhotonNetwork.Instantiate())
        List<GameObject> randomizedSpawners = new List<GameObject>();
        foreach (GameObject spawner in itemSpawners)
        {
            int r = UnityEngine.Random.Range(0, randomizedSpawners.Count);
            randomizedSpawners.Insert(r, spawner);
        }

        Debug.Log("Original Spawner List");
        foreach (GameObject spawner in itemSpawners)
        {
            Debug.Log(spawner.name);
        }
        Debug.Log("Randomized Spawner List");
        foreach (GameObject spawner in randomizedSpawners)
        {
            Debug.Log(spawner.name);
        }
    }

    public void CloseClient()
    {
        ClientManager.Instance.CloseClient();
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
