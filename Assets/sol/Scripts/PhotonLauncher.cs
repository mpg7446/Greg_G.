using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using UnityEngine.Rendering;
using WebSocketSharp;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    public static PhotonLauncher Instance;

    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private List<TMP_Text> roomNameDisplays;
    void Start()
    {
        Instance = this;

        Debug.Log("Photon: Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();

        if (roomNameInput == null)
        {
            Debug.LogError("Error: no roomNameInput selected");
        }
    }

    public override void OnConnectedToMaster() // On Connected to Master Network
    {
        Debug.Log("Photon: Successfully Joined Master!");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby() // On Joined Lobby
    {
        Debug.Log("Photon: Successfully Joined Lobby!");
        MenuManager.Instance.OpenMenu("main");
    }

    public void CreateRoom(string roomName) // Create room with custom room name
    {
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogWarning("Unable to create room: No room name given");
            return;
        }
        PhotonNetwork.CreateRoom(roomName);
        MenuManager.Instance.OpenMenu("loading");
    }
    public void CreateRoom() // Create room with generated room name
    {
        PhotonNetwork.CreateRoom(GenerateRandomName()); // Create room with new hex code as room name
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom()
    {
        if (roomNameInput != null && roomNameInput.text != "")
        {
            string input = roomNameInput.text.ToUpper();
            Debug.Log("Attempting to join room " + input);
            JoinRoom(input);
        }
    }

    public void JoinRoom(string roomName) // Join room with room name
    {
        if (!roomName.IsNullOrEmpty())
        {
            PhotonNetwork.JoinRoom(roomName);
            MenuManager.Instance.OpenMenu("loading");
        }
        else // Room name incorrectly entered
        {
            MenuManager.Instance.OpenMenu("joinRooms");
            roomNameInput.text = null;
        }
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
        ClientManager.Instance.CloseScene("Lobby");
    }

    public override void OnJoinedRoom() // On joined room - also calls after creating room
    {
        Debug.Log("Photon: Successfully Joined Room " + PhotonNetwork.CurrentRoom.Name);

        MenuManager.Instance.OpenMenu("room");
        ClientManager.Instance.LoadScene("Lobby");
        
        // update room code visuals
        foreach (TMP_Text roomNameDisplay in roomNameDisplays)
        {
            roomNameDisplay.text = "Lobby Code: " + PhotonNetwork.CurrentRoom.Name;
        }

        // spawn network player
        SpawnPlayer("Menu Player");
    }

    public override void OnJoinRoomFailed(short returnCode, string message) // Failed to connect to room, probably doesnt exist
    {
        MenuManager.Instance.OpenMenu("joinRooms");
        roomNameInput.text = "lobby not dounf";
    }
    public override void OnLeftRoom() // Return to menus when leaving room
    {
        MenuManager.Instance.OpenMenu("main");
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("Photon: Successfully Left Room");
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message) // If joined room fails
    {
        Debug.LogWarning("Photon Create Room Error: " + message);
        MenuManager.Instance.OpenMenu("main");
    }

    private string GenerateRandomName()
    {
        int add = PhotonNetwork.CountOfRooms * 10;
        string roomID = Random.Range(0, 9) + add.ToString("X");

        add = PhotonNetwork.CountOfPlayers * 10;
        roomID += Random.Range(0, 9) + add.ToString("X");

        return roomID;
    }

    public void SpawnPlayer(string playerType)
    {
        PhotonNetwork.Instantiate(playerType, Vector3.zero, Quaternion.identity);
    }
    public void SpawnPlayer(string playerType, Vector3 position)
    {
        PhotonNetwork.Instantiate(playerType, position, Quaternion.identity);
    }
}
