using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private List<TMP_Text> roomNameDisplays;

    void Start()
    {
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
        PhotonNetwork.CreateRoom(GenerateRandomName(7, 3)); // Create room with new hex code as room name
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
        if (roomName != null && roomName != "")
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
        
        foreach (TMP_Text roomNameDisplay in roomNameDisplays)
        {
            roomNameDisplay.text = "Lobby Code: " + PhotonNetwork.CurrentRoom.Name;
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        MenuManager.Instance.OpenMenu("joinRooms");
        roomNameInput.text = null;
    }
    public override void OnLeftRoom()
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

    private string GenerateRandomName(int mult, int add)
    {
        int roomID = mult * (PhotonNetwork.CountOfRooms + add);

        return roomID.ToString("X");
    }
}
