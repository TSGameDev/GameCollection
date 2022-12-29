using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomButtonTitleText;
    [SerializeField] TextMeshProUGUI lobbyRoomTitleDisplay;
    [SerializeField] TextMeshProUGUI lobbyRoomSettingsDisplay;
    [SerializeField] Button joinRoomButton;

    Button roomButton;

    private void Awake()
    {
        roomButton = GetComponent<Button>();    
    }

    public void Reinitialise(RoomData roomData)
    {
        roomButtonTitleText.text = roomData.roomName;
        roomButton.onClick.AddListener(() => 
        {
            lobbyRoomTitleDisplay.text = roomData.roomName;
            lobbyRoomSettingsDisplay.text += $"Max Players: {roomData.maxPlayers}";
            lobbyRoomSettingsDisplay.text += $"Game Mode: {roomData.gameMode}";
            joinRoomButton.onClick.AddListener(() => { PhotonNetwork.JoinRoom(roomData.roomName); });
        });
    }

}
