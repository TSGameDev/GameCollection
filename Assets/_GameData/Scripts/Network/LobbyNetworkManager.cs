using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Photon.Pun;
using Photon.Realtime;

public enum EGameMode
{
    DeliveryWithTheDead,
    Collector,
    OneInTheChamber
}

public struct RoomData
{
    public int maxPlayers;
    public EGameMode gameMode;
    //public string hostName;
    public string roomName;

    public RoomData(string roomName, EGameMode gameMode, int maxPlayers)
    {
        this.maxPlayers = maxPlayers;
        this.gameMode = gameMode;
        //this.hostName = hostName;
        this.roomName = roomName;
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
    [SerializeField] GameObject roomItemContent;

    #endregion

    #region Life Cycle

    private void Start()
    {
        PhotonNetwork.JoinLobby();
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
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable()
            {
                {"gameMode", selectedGameMode}
            };

            RoomOptions roomOptions = new()
            {
                MaxPlayers = (byte)selectedMaxPlayers,
                CustomRoomProperties = customProperties
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

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RoomListUpdate(roomList);
    }

    #endregion

    #region Private Functions

    private void RoomSettingSetup()
    {
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
    }

    private void RoomListUpdate(List<RoomInfo> roomList)
    {
        List<RoomData> activeRoomDatas = new();

        //loop throug all room infos
        foreach (RoomInfo roominfo in roomList)
        {
            //create roomdata for each activeRoom
            RoomData newRoom = new(roominfo.Name, (EGameMode)roominfo.CustomProperties["gameMode"], roominfo.MaxPlayers);
            activeRoomDatas.Add(newRoom);
        }


        //loop throug all room button objects
        for(int i = 0; i > roomItemObjects.Count - 1; i++)
        {
            //if there are more rooms than buttons spawn more buttons
            if(roomItemObjects.Count < activeRoomDatas.Count)
            {
                int spawnButtonAmount = activeRoomDatas.Count - roomItemObjects.Count;

                for(int x = 0; x > spawnButtonAmount; x++)
                {
                    GameObject newRoomItem = Instantiate(roomItemPrefab, roomItemContent.transform);
                    RoomItem newRoomItemComp = newRoomItem.GetComponent<RoomItem>();
                    roomItemObjects.Add(newRoomItemComp);
                }
            }

            //deactive all room button objects
            roomItemObjects[i].gameObject.SetActive(false);

            //Call the Reinitialise function passing in roomdata
            roomItemObjects[i].Reinitialise(activeRoomDatas[i]);
        }
    }

    #endregion
}
