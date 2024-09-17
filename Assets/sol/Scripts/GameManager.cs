using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private PhotonView photonView;

    public bool enablePickupCountdown = true;
    public bool itemsRunAway = false;

    public int playerCount = 0;

    // item counts
    public int maxItems;
    public int currentItems;
    public List<GameObject> itemSpawners;

    public Vector3 spawnLocation;

    // TESTING ONLY!!
    // DO NOT USE IN FINAL BUILD (for now)
    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

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
            Debug.Log("GameManager: Game Ended");
            EndGame();
        }
    }

    public void StartGame(bool RPCHost = false)
    {
        Debug.Log("GameManager: Game Started");

        if (RPCHost)
        {
            photonView.RPC("RPCStartGame",RpcTarget.Others);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            SpawnItems();
        }

        PlayerManager.Instance.DestroyPlayer();

        PhotonLauncher.Instance.SpawnPlayer("Player", spawnLocation);
        MenuManager.Instance.OpenMenu("game");
        ClientManager.Instance.LoadScene("Empty Environment", "Lobby");
        ClientManager.Instance.GameStarted();

        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    [PunRPC]
    private void RPCStartGame()
    {
        Debug.Log("Remote user started game!");
        StartGame();
    }

    private void SpawnItems()
    {
        // Spawn items over network (PhotonNetwork.Instantiate())
        List<GameObject> randomizedSpawners = new List<GameObject>();
        foreach (GameObject spawner in itemSpawners)
        {
            int r = Random.Range(0, randomizedSpawners.Count);
            randomizedSpawners.Insert(r, spawner);
        }


        // Place items in randomized list
        int itemCount = 0;
        foreach (GameObject spawner in randomizedSpawners)
        {
            if (itemCount < maxItems)
            {
                PhotonNetwork.Instantiate("Test Item", spawner.transform.position, Quaternion.identity);
                itemCount++;
            }
        }
    }

    public void CloseClient()
    {
        ClientManager.Instance.CloseClient();
    }

    public void EndGame(bool RPC = false)
    {
        Debug.Log("GameManager: Game Ended");

        if (RPC)
        {
            photonView.RPC("RPCEndGame", RpcTarget.Others);
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }

        MenuManager.Instance.OpenMenu("lobby");
        ClientManager.Instance.LoadScene("Lobby", "Empty Environment");

        PlayerManager.Instance.DestroyPlayer();
        PhotonLauncher.Instance.SpawnPlayer("Menu Player");
        ClientManager.Instance.GameFinished();
    }

    [PunRPC] 
    private void RPCEndGame()
    {
        Debug.Log("Remote user ended game!");
        EndGame();
    }

    public void PlayerLeft(Player player)
    {
        if (playerCount <= 1 && ClientManager.Instance.gameRunning)
        {
            EndGame(true);
        }
    }
}
