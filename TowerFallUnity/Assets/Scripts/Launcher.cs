using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Button _connectionButton;
    [SerializeField]
    private TMPro.TextMeshProUGUI _feedbackText;
    
    private bool _isConnecting;
    private string _gameVersion = "1";
    private byte _maxNumberOfPlayers = 3;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void Connect()
    {
        _feedbackText.text = "";

        _connectionButton.interactable = false;

        _isConnecting = true;

        if (PhotonNetwork.IsConnected)
        {
            LogFeedback("Joining Room...");

            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            LogFeedback("Connecting...");

            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = this._gameVersion;
        }
    }

    private void LogFeedback(string message)
    {
        if (_feedbackText == null)
        {
            return;
        }

        _feedbackText.text += System.Environment.NewLine + message;
    }

    public override void OnConnectedToMaster()
    {
        if (_isConnecting)
        {
            LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");

            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        LogFeedback("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room");
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this._maxNumberOfPlayers });
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        LogFeedback("<Color=Red>OnDisconnected</Color> " + cause);
        Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");

        _isConnecting = false;
        _connectionButton.interactable = true;

    }

    public override void OnJoinedRoom()
    {
        LogFeedback("<Color=Green>OnJoinedRoom</Color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("We load the 'Room for 1' ");

            PhotonNetwork.LoadLevel("PunBasics-Room for 1");
        }
    }
}
