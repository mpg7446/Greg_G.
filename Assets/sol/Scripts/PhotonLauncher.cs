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
    }

    public override void OnConnectedToMaster() // On Connected to Master Network
    {
        Debug.Log("Photon: Successfully Joined Master!");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby() // On Joined Lobby
    {
        Debug.Log("Photon: Successfully Joined Lobby!");
        MenuManager.instance.OpenMenu("main");
    }

    public void CreateRoom(string roomName) // Create room with custom room name
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            Debug.LogWarning("Unable to create room: No room name given");
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInput.text);
        MenuManager.instance.OpenMenu("loading");
    }
    public void CreateRoom() // Create room with generated room name
    {
        PhotonNetwork.CreateRoom(GenerateRandomName()); // Create room with new hex code as room name
        MenuManager.instance.OpenMenu("loading");
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
            MenuManager.instance.OpenMenu("loading");
        }
        else // Room name incorrectly entered
        {
            MenuManager.instance.OpenMenu("joinRooms");
            roomNameInput.text = null;
        }
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom() // On joined room - also calls after creating room
    {
        Debug.Log("Photon: Successfully Joined Room " + PhotonNetwork.CurrentRoom.Name);
        MenuManager.instance.OpenMenu("room");
        
        foreach (TMP_Text roomNameDisplay in roomNameDisplays)
        {
            roomNameDisplay.text = "Lobby Code: " + PhotonNetwork.CurrentRoom.Name;
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        MenuManager.instance.OpenMenu("joinRooms");
        roomNameInput.text = null;
    }
    public override void OnLeftRoom()
    {
        MenuManager.instance.OpenMenu("main");
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("Photon: Successfully Left Room");
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message) // If joined room fails
    {
        Debug.LogWarning("Photon Create Room Error: " + message);
        MenuManager.instance.OpenMenu("main");
    }

    private string GenerateRandomName()
    {
        int roomID = 23 * (PhotonNetwork.CountOfRooms + 3);

        return roomID.ToString("X");
    }
}
