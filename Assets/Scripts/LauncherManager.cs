using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExitGames.Client.Photon;

using ExitGames.Client.Photon;

public class LauncherManager : PunBehaviour
{
    public InputField InputField;
    public string UserId;

    public GameObject startMenuCanvas;
    public GameObject launchMenuCanvas;
    string previousRoomPlayerPrefKey = "PUN:Demo:RPS:PreviousRoom";
    public string previousRoom;

    private const string MainSceneName = "TicTacGo";
    const string NickNamePlayerPrefsKey = "NickName";

    Hashtable _defaultRoomProperties = new Hashtable
    {
        { "BoardSize", 3 }
    };

    
    void Start()
    {
        InputField.text = PlayerPrefs.HasKey(NickNamePlayerPrefsKey) ? PlayerPrefs.GetString(NickNamePlayerPrefsKey) : "";
    }

    public void ApplyUserIdAndConnect()
    {
        string nickName = "DemoNick";
        if (this.InputField != null && !string.IsNullOrEmpty(this.InputField.text))
        {
            nickName = this.InputField.text;
            PlayerPrefs.SetString(NickNamePlayerPrefsKey, nickName);
        }

        if (PhotonNetwork.AuthValues == null)
        {
            PhotonNetwork.AuthValues = new AuthenticationValues();
        }

        PhotonNetwork.AuthValues.UserId = nickName;

        Debug.Log("Nickname: " + nickName + " userID: " + this.UserId, this);


        PhotonNetwork.playerName = nickName;
        PhotonNetwork.ConnectUsingSettings("0.5");

        // this way we can force timeouts by pausing the client (in editor)
        PhotonHandler.StopFallbackSendAckThread();
    }

    public override void OnConnectedToMaster()
    {
        // after connect 
        this.UserId = PhotonNetwork.player.UserId;
        ////Debug.Log("UserID " + this.UserId);
        if (PlayerPrefs.HasKey(previousRoomPlayerPrefKey))
        {
            Debug.Log("getting previous room from prefs: ");
            this.previousRoom = PlayerPrefs.GetString(previousRoomPlayerPrefKey);
            PlayerPrefs.DeleteKey(previousRoomPlayerPrefKey); // we don't keep this, it was only for initial recovery
        }
        // after timeout: re-join "old" room (if one is known)
        if (!string.IsNullOrEmpty(this.previousRoom))
        {
            Debug.Log("ReJoining previous room: " + this.previousRoom);
            PhotonNetwork.ReJoinRoom(this.previousRoom);
            this.previousRoom = null;       // we only will try to re-join once. if this fails, we will get into a random/new room
        }

        startMenuCanvas.SetActive(true);
        launchMenuCanvas.SetActive(false);

    }

    public override void OnJoinedLobby()
    {
        OnConnectedToMaster(); // this way, it does not matter if we join a lobby or not
    }

    public void LaunchRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("OnPhotonRandomJoinFailed");
<<<<<<< HEAD

        RoomOptions roomOptions = new RoomOptions();     
        roomOptions.CustomRoomProperties = new Hashtable() { { "mapSize", 3 } };
        roomOptions.MaxPlayers = 2;
        roomOptions.PlayerTtl = 20000;

        PhotonNetwork.CreateRoom(null, roomOptions, null);
=======
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2, PlayerTtl = 20000}, null);
>>>>>>> 468957a470108fbe56554726e69acaa8bd75bdd2
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.room.Name);
        this.previousRoom = PhotonNetwork.room.Name;
        PlayerPrefs.SetString(previousRoomPlayerPrefKey, this.previousRoom);
<<<<<<< HEAD

        if (!PhotonNetwork.isMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        Debug.Log("PhotonNetwork : Loading Level : " + "TikTakGo");
        PhotonNetwork.LoadLevel("TicTacGo");
=======
        if(PhotonNetwork.isMasterClient)
            PhotonNetwork.room.SetCustomProperties(_defaultRoomProperties);
>>>>>>> 468957a470108fbe56554726e69acaa8bd75bdd2
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        Debug.Log("OnPhotonJoinRoomFailed");
        this.previousRoom = null;
        PlayerPrefs.DeleteKey(previousRoomPlayerPrefKey);
    }

    public override void OnConnectionFail(DisconnectCause cause)
    {
        Debug.Log("Disconnected due to: " + cause + ". this.previousRoom: " + this.previousRoom);
    }

    public override void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer)
    {
        Debug.Log("OnPhotonPlayerActivityChanged() for " + otherPlayer.NickName + " IsInactive: " + otherPlayer.IsInactive);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
