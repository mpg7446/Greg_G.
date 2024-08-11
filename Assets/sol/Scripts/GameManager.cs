using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
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
            Debug.Log("GAME ENDED GOAL WOOOOHHHH YEAHH BABYYYYYYY");
        }
    }

    public void StartGame(bool RPC = false)
    {
        Debug.Log("Game Started");

        if (RPC)
        {
            photonView.RPC("RPCStartGame",RpcTarget.Others);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            SpawnItems();
        }

        MenuManager.Instance.OpenMenu("empty");
        ClientManager.Instance.LoadScene("Empty Environment", "Lobby");
        PlayerMovement.Instance.DestroyPlayer();

        ClientManager.Instance.GameStarted();
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
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }

        MenuManager.Instance.OpenMenu("Lobby");
        ClientManager.Instance.LoadScene("Lobby", "sol nainofdsi sdfg");
        PlayerMovement.Instance.DestroyPlayer();

        ClientManager.Instance.GameFinished();
    }

    [PunRPC] 
    private void RPCEndGame()
    {
        Debug.Log("Remote user ended game!");
        EndGame();
    }
}
