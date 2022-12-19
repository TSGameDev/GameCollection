using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public enum EGameMode
{
    DeliveryWithTheDead,
    Collector,
    OneInTheChamber
}

public struct RoomData
{
    public RoomInfo roomInfo;
    public EGameMode selectedGameMode;
    public int selectedMaxPlayers;
    public string hostName;

    public RoomData(RoomInfo roomInfo, EGameMode selectedGameMode, int selectedMaxPlayers, string hostName)
    {
        this.roomInfo = roomInfo;
        this.selectedGameMode = selectedGameMode;
        this.selectedMaxPlayers = selectedMaxPlayers;
        this.hostName = hostName;
    }
}

public class LobbyNetworkManager : MonoBehaviourPunCallbacks
{
    #region Room Creation Variables

    [SerializeField] TMP_InputField roomNameInput;
    [SerializeField] TMP_InputField roomPasswordInput;
    [SerializeField] TMP_Dropdown selectedGameInput;
    [SerializeField] TMP_Dropdown maxPlayersInput;

    [SerializeField] TextMeshProUGUI roomNameTxt;
    [SerializeField] TextMeshProUGUI roomSettingsTxt;

    private EGameMode selectedGameMode = EGameMode.DeliveryWithTheDead;
    private int selectedMaxPlayers = 1;

    #endregion

    #region Lobby Population Variable

    [SerializeField] GameObject roomItemPrefab;
    [SerializeField] List<RoomItem> roomItemObjects;
    [SerializeField] List<RoomData> activeRoomDatas = new();
    [SerializeField] GameObject roomItemContent;

    TypedLobby typedLobby = new("Default", LobbyType.Default);

    #endregion

    #region Life Cycle

    private PhotonView photonView;
    private void Start()
    {
        PhotonNetwork.JoinLobby(typedLobby);
        photonView = PhotonView.Get(this);
    }

    #endregion

    #region EventFunctions

    public void OnDropdownGameSelect(int index)
    {
        switch(index)
        {
            case 0: 
                selectedGameMode = EGameMode.DeliveryWithTheDead; 
                break;
            case 1: 
                selectedGameMode = EGameMode.Collector; 
                break;
            case 2: 
                selectedGameMode = EGameMode.OneInTheChamber; 
                break;
        }
        Debug.Log(Enum.GetName(typeof(EGameMode), selectedGameMode));
    }

    public void OnDropdownPlayerCountSelect(int index)
    {
        int playerCount = index + 1;
        selectedMaxPlayers = playerCount;
        Debug.Log(selectedMaxPlayers);
    }

    public void OnClickCreateRoom()
    {
        if(roomNameInput.text.Length >= 3)
        {
            RoomOptions roomOptions = new()
            {
                MaxPlayers = (byte)selectedMaxPlayers
            };

            PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
        }
    }

    public void OnClickLeave()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Public Functions

    public override void OnJoinedRoom()
    {
        RoomSettingSetup();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log($"Room List Updated");
        RoomListUpdate(roomList);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(typedLobby);
    }
    
    #endregion

    #region Private Functions

    private void RoomSettingSetup()
    {
        Debug.Log(PhotonNetwork.CurrentRoom);
        string newline = Environment.NewLine;

        //Sets room title text
        roomNameTxt.text = "";
        roomNameTxt.text += $"{PhotonNetwork.CurrentRoom.Name}";

        //collects room host name
        int hostID = PhotonNetwork.CurrentRoom.MasterClientId;
        string hostName = PhotonNetwork.CurrentRoom.GetPlayer(hostID).NickName;

        //sets room settings text to all the host chosen settings
        roomSettingsTxt.text = "";
        roomSettingsTxt.text += $"Host: {hostName}{newline}";
        roomSettingsTxt.text += $"Selected Game: {Enum.GetName(typeof(EGameMode), selectedGameMode)}{newline}";
        roomSettingsTxt.text += $"Max Players: {selectedMaxPlayers}{newline}";
        roomSettingsTxt.text += $"Password: ????";

        photonView.RPC("RoomDataCreation", RpcTarget.All, hostName);
    }

    [PunRPC]
    private void RoomDataCreation(string hostName)
    {
        RoomData newRoomData = new RoomData(PhotonNetwork.CurrentRoom, selectedGameMode, selectedMaxPlayers, hostName);
        activeRoomDatas.Add(newRoomData);

        Debug.Log($"Room {newRoomData.roomInfo.Name} created. New count of active room datas {activeRoomDatas.Count}");
    }

    private void RoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log(activeRoomDatas.Count);
        //Check activeRoomData is up-to-date on all rooms in roomlist (remove closed rooms from activeRoomData)
        if(activeRoomDatas.Count > roomList.Count)
        {
            Debug.Log("Trimming active room datas");
            foreach(RoomData roomdata in activeRoomDatas)
            {
                foreach (RoomInfo roominfo in roomList)
                {
                    if (roomdata.roomInfo.Name == roominfo.Name)
                        return;

                    if(roominfo == roomList.Last())
                    {
                        Debug.Log($"Removing {roomdata.roomInfo.Name} Room");
                        activeRoomDatas.Remove(roomdata);
                    }
                }
            }
        }


        //Check enough room button objects exists
        if (activeRoomDatas.Count > roomItemObjects.Count)
        {
            Debug.Log($"Spawning more room item objects");
            int activeRoomAmount = activeRoomDatas.Count;
            int roomObjectAmount = roomItemObjects.Count;
            int difference = activeRoomAmount - roomObjectAmount;

            for(int i = 0; i < difference; i++)
            {
                GameObject newRoomButton = Instantiate(roomItemPrefab, roomItemContent.transform);
                newRoomButton.SetActive(false);
            }
        }

        //Handle room button objects
        for(int i = 0; i < activeRoomDatas.Count - 1; i++)
        {
            Debug.Log($"Initialising room item objects");
            //deactive all room button objects
            roomItemObjects[i].gameObject.SetActive(false);
            //Reinitialise the room buttons
            roomItemObjects[i].Reinitialise(activeRoomDatas[i]);
        }
    }

    #endregion
}
