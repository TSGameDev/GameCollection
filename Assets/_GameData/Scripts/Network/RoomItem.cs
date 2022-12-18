using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomButtonTitleText;
    [SerializeField] TextMeshProUGUI lobbyRoomTitleDisplay;
    [SerializeField] TextMeshProUGUI lobbyRoomSettingsDisplay;

    Button roomButton;

    private void Awake()
    {
        roomButton = GetComponent<Button>();    
    }

    public void Reinitialise(RoomInfo roomInfo)
    {
        //reset all the data to the passed in data
        //reenable the object
    }

}
