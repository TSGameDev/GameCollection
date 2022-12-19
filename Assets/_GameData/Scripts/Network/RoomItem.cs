using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomButtonTitleText;
    [SerializeField] TextMeshProUGUI lobbyRoomTitleDisplay;
    [SerializeField] TextMeshProUGUI lobbyRoomSettingsDisplay;

    Button roomButton;
    string newLine = Environment.NewLine;


    private void Awake()
    {
        roomButton = GetComponent<Button>();    
    }

    public void Reinitialise(RoomData roomData)
    {
        //reset all the data to the passed in data
        roomButtonTitleText.text = roomData.roomInfo.Name;

        roomButton.onClick.AddListener(() => 
        {
            lobbyRoomTitleDisplay.text = "";
            lobbyRoomTitleDisplay.text = roomData.roomInfo.Name;
            lobbyRoomSettingsDisplay.text= "";
            lobbyRoomSettingsDisplay.text += $"Host Name: {roomData.hostName}{newLine}";
            lobbyRoomSettingsDisplay.text += $"Game Selected: {roomData.selectedGameMode}{newLine}";
            lobbyRoomSettingsDisplay.text += $"Max Players: {roomData.selectedMaxPlayers}{newLine}";
        });

        //reenable the object
        gameObject.SetActive(true);
        Debug.Log($"Initialsed {gameObject.name}");
    }

}
