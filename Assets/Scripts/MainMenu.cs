using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : Photon.MonoBehaviour {
    private string GameScene = "Game";

    #region OnClick Events
    public void OnClick_Play()
    {
        StartLocalGame();
    }
    public void OnClick_Online()
    {
        StartOnlineGame();
    }
    public void OnClick_Customize()
    {

    }
    public void OnClick_Share()
    { 

    }
    #endregion


    private void StartLocalGame()
    {
        SceneManager.LoadScene(GameScene);
    }
    private void StartOnlineGame()
    {
        GetComponent<ConnectAndJoinRandom>().AutoConnect = true;
    }

    public virtual void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(GameScene);
    }
}
