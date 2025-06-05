//#define On

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public GameObject spawnPoint;

    private void Awake()
    {
        PhotonNetwork.NickName = "Ban_si";
        PhotonNetwork.GameVersion = "1.0";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
#if On
        Debug.Log(PhotonNetwork.SendRate);
#endif
    }

    public override void OnConnectedToMaster()
    {
#if On
        Debug.Log("Connected to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
#endif
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
#if On
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
#endif
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
#if On
        Debug.Log($"JoinRoom Faild {returnCode}:{message}");
#endif
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;
        PhotonNetwork.CreateRoom("My Room", ro);
    }

    public override void OnCreatedRoom()
    {
#if On
    Debug.Log("Created Room");
    Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
#endif
    }

    public override void OnJoinedRoom()
    {
#if On
    Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
    Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
#endif
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            //Debug.Log($"{player.Value.NickName},{player.Value.ActorNumber}");
        }
        PhotonNetwork.Instantiate("Rio", spawnPoint.transform.position, spawnPoint.transform.rotation, 0);
    }
}
