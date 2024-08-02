using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField roomNameInput;
    void Start()
    {
        Debug.Log("Photon: Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() // On Connected to Master Network
    {
        Debug.Log("Photon: Successfully Joined Master!");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() // On Joined Lobby
    {
        Debug.Log("Photon: Successfully Joined Lobby!");
        MenuManager.instance.OpenMenu("main");
    }

    public void CreateRoom(string roomName)
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            Debug.LogWarning("Unable to create room: No room name given");
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInput.text);
        MenuManager.instance.OpenMenu("loading");
    }
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null);
        MenuManager.instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Photon: Successfully Joined Room " + PhotonNetwork.CurrentRoom.Name);
        MenuManager.instance.OpenMenu("joinRooms");
    }
    public override void OnLeftRoom()
    {
        MenuManager.instance.OpenMenu("main");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {

    }
}
