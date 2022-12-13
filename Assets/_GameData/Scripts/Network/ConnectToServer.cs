using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField usernameInput;
    [SerializeField] TextMeshProUGUI connectingText;

    public void OnClickConnect()
    {
        if (usernameInput.text.Length >= 3)
        {
            PhotonNetwork.NickName = usernameInput.text;
            connectingText.text = "Connecting...";
            PhotonNetwork.ConnectUsingSettings();
        }
        else
            connectingText.text = "Name to short, name needs to be a minimum of 3 characters";
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene(1);
    }
}
