using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private PhotonView photonView;

    public string map;

    public bool enablePickupCountdown = true;
    public bool itemsRunAway = false;

    public bool IsRunning {  get; private set; }
    public int playerCount = 0;

    // Item Management
    [SerializeField] private int maxItems;
    [SerializeField] private int itemsPerPlayer;
    public int currentItems { get; private set; }
    public bool PassedMaxItems { get { return currentItems >= maxItems; } }
    //public int currentItems { get { return items.Count; } }
    //private List<GameObject> items;
    public List<GameObject> itemSpawners;

    public Vector3 spawnLocation;

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
    /*
    public void PickupItem(int amount = 1)
    {
        currentItems -= amount;
        //Debug.Log("GameManager: PickupItem called with amount " + amount);

        if (currentItems >= maxItems)
        {
            Debug.Log("GameManager: Game Ended");
            EndGame(true);
        }
    }
    public void PickupItem(PlayerInventory sender, int amount = 1)
    {
        currentItems = currentItems + amount;
        Debug.Log("GameManager: ItemCount changed to " + currentItems);
        if (sender.IsMine && currentItems >= maxItems)
        {
            Debug.Log("GameManager: Game Ended by sender");
            EndGame(true);
        }
    }*/

    [PunRPC]
    public void StartGame(bool rpc = false) // Called by Start button in menu
    {
        DestroyItems();
        Debug.Log("GameManager: Game Started");

        if (rpc)
        {
            photonView.RPC("StartGame",RpcTarget.Others, false);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            SpawnItems();
        }

        PlayerManager.Instance.DestroyPlayer();

        PhotonLauncher.Instance.SpawnPlayer("Player", spawnLocation);
        MenuManager.Instance.OpenMenu("game");
        ClientManager.Instance.LoadScene(map, "Lobby");
        ClientManager.Instance.GameStarted();

        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        IsRunning = true;
        currentItems = 0;
    }
    /*
    [PunRPC]
    private void RPCStartGame()
    {
        Debug.Log("Remote user started game!");
        StartGame();
    }*/

    private void SpawnItems()
    {
        // Set items per player if enabled
        if (itemsPerPlayer > 0)
            maxItems = itemsPerPlayer * PhotonNetwork.CurrentRoom.PlayerCount;

        // Cap item count to count of itemspawners
        if (maxItems > itemSpawners.Count)
            maxItems = itemSpawners.Count;

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
    public void DestroyItems()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        for (int i = 0; i < items.Length; i++)
        {
            Destroy(items[i]);
        }
    }
    /*
    private void ClearItems()
    {
        foreach (GameObject item in items)
            PhotonNetwork.Destroy(item);
        items = new List<GameObject>();
    }

    public void RemoveItem(GameObject obj)
    {
        if (items.Contains(obj))
            items.Remove(obj);
        else
            foreach (GameObject item in items)
                if (item == null)
                    items.Remove(item);
    }*/
    public void IncreaseItems(int amount = 1)
    {
        currentItems += amount;
    }

    public void CloseClient()
    {
        ClientManager.Instance.CloseClient();
    }
    /*
    [PunRPC]
    private void RPCEndGame()
    {
        Debug.Log("Remote user ended game!");
        EndGame(false);
    }*/

    [PunRPC]
    public void EndGame(bool rpc = false)
    {
        if (IsRunning) {
            Debug.Log("GameManager: Ending Game");

            if (rpc)
            {
                photonView.RPC("EndGame", RpcTarget.Others, false);
                PhotonNetwork.CurrentRoom.IsOpen = true;
                //ClearItems();
            }

            // Check if player won
            if (ScoreCounter.Instance.winner.playerID == PlayerManager.Instance.ID)
            {
                MenuManager.Instance.OpenMenu("win");
                ClientManager.Instance.wonLastRound = true;
            }
            else
            {
                MenuManager.Instance.OpenMenu("lose");
                ClientManager.Instance.wonLastRound = false;
            }

            // Reset counters and scores and players and more scores :3
            ClientManager.Instance.CloseScene(map);
            ScoreCounter.Instance.ClearCounters();
            PlayerManager.Instance.DestroyPlayer();
            ClientManager.Instance.GameFinished();
            IsRunning = false;

            // Call timer event to close win/lose screen
            StartCoroutine(closeStateScreen(4.2f));
        }

        // Destroy all existing items in case of errors rejoining game
        DestroyItems();
    }

    private IEnumerator closeStateScreen(float time)
    {
        Debug.Log($"Started CloseStateScreen Timer for {time}s");
        yield return new WaitForSeconds(time);

        Debug.Log("Finished CloseStatScreen Timer");
        MenuManager.Instance.OpenMenu("lobby", true);
        ClientManager.Instance.LoadScene("Lobby");
        PhotonLauncher.Instance.SpawnPlayer("Menu Player");
    }

    public void PlayerLeft(Player player)
    {
        if (playerCount <= 1 && ClientManager.Instance.gameRunning)
        {
            EndGame(true);
        }
    }
}
