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

    private Dictionary<string, RoomInfo> _cachedRoomList = new Dictionary<string, RoomInfo>();

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

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                if (_cachedRoomList.ContainsKey(room.Name))
                {
                    _cachedRoomList.Remove(room.Name);
                }
            }
            else
            {
                Debug.Log("Room added/updated: " + room.Name);
                _cachedRoomList[room.Name] = room;
            }
        }
    }

    private string GenerateRandomName()
    {
        int roomID = 023 + PhotonNetwork.CountOfRooms;

        return roomID.ToString("X");
    }

    private int ConvertToInt(string name)
    {
        return int.Parse(name, System.Globalization.NumberStyles.HexNumber);
    }
    private string ConvertToHex(int name)
    {
        return name.ToString("X");
    }
}
