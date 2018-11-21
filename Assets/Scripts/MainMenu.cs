using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : Photon.MonoBehaviour , IScreen{
    private string GameScene = "Game";
    GameOptions options;
    IScreen optionsMenu;

    private void Awake()
    {
        optionsMenu = FindObjectOfType<OptionsMenu>();
        options = FindObjectOfType<GameOptions>();
        
    }
    public void Show()
    {
        GetComponent<Canvas>().enabled = true;
    }
    public void Hide()
    {
        GetComponent<Canvas>().enabled = false;
    }
    #region OnClick Events
    public void OnClick_Play()
    {
        StartGame();
    }
    public void OnClick_Bot()
    {
        options.IsBotGame = true;
        options.IsOnlineGame = false;
        ShowGameOptions();
    }
    public void OnClick_Local()
    {
        options.IsOnlineGame = false;
        options.IsBotGame = false;
        ShowGameOptions();
    }
    public void OnClick_Online()
    {
        options.IsOnlineGame = true;
        options.IsBotGame = false;
        ShowGameOptions();
    }
    public void OnClick_Customize()
    {

    }
    #endregion

    private void ShowGameOptions()
    {
        Hide();
        optionsMenu.Show();
    }
    private void StartGame()
    {
        if(options.IsOnlineGame)
            StartOnlineGame();
        else
            StartLocalGame();
        // Needs bot game check
    }
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
