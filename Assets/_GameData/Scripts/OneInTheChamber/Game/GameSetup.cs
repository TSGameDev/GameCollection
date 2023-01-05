using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

namespace TSGameDev
{
    public class GameSetup : MonoBehaviourPunCallbacks
    {
        [SerializeField] GameObject playerPrefab;
        [SerializeField] GameObject[] spawnPoints;

        private void Awake()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            PhotonNetwork.CreateRoom("DevRoom");
        }

        public override void OnJoinedRoom()
        {
            int randomNum = Random.Range(0, spawnPoints.Length);
            PhotonNetwork.InstantiateRoomObject("TestPlayer", spawnPoints[randomNum].transform.position, Quaternion.identity);
        }
    }
}
