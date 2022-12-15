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

public class LobbyNetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField roomNameInput;
    [SerializeField] TMP_InputField roomPasswordInput;
    [SerializeField] TMP_Dropdown selectedGameInput;
    [SerializeField] TMP_Dropdown maxPlayersInput;

    [SerializeField] TextMeshProUGUI roomNameTxt;
    [SerializeField] TextMeshProUGUI roomSettingsTxt;

    private EGameMode selectedGameMode = EGameMode.DeliveryWithTheDead;
    private int selectedMaxPlayers = 1;

    private void Start()
    {
        PhotonNetwork.JoinLobby();
    }

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

    public override void OnJoinedRoom()
    {
        string newline = Environment.NewLine;

        //Change room layout to selected settings. I.E. room name text to room name

        roomNameTxt.text = "";
        roomNameTxt.text += $"{PhotonNetwork.CurrentRoom.Name}";

        int hostID = PhotonNetwork.CurrentRoom.MasterClientId;
        string hostName = PhotonNetwork.CurrentRoom.GetPlayer(hostID).NickName;
        roomSettingsTxt.text = "";
        roomSettingsTxt.text += $"Host: {hostName}{newline}";
        roomSettingsTxt.text += $"Selected Game: {Enum.GetName(typeof(EGameMode), selectedGameMode)}{newline}";
        roomSettingsTxt.text += $"Max Players: {selectedMaxPlayers}{newline}";
        roomSettingsTxt.text += $"Password: ????";

    }

    public void OnClickLeave()
    {
        PhotonNetwork.LeaveRoom();
    }
}
