using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class MainMenu : Photon.MonoBehaviour , IScreen{
    GameOptions options;
    IScreen optionsMenu;
    private string GameScene { get { return options.Start3D ? "Game_3D" : "Game"; } }
    [SerializeField]
    Button btnToggleBoard;
    [SerializeField]
    Button btnToggleMode;
    [SerializeField]
    Button btnPlay;

    private void Start()
    {
        optionsMenu = FindObjectOfType<OptionsMenu>();
        options = FindObjectOfType<GameOptions>();
        btnToggleBoard.onClick.AddListener(OnClick_ToggleBoard);
        btnToggleMode.onClick.AddListener(OnClick_ToggleMode);
        btnPlay.onClick.AddListener(OnClick_Play);
        SetBoardSelectSprite();
        SetModeSprite();
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
    public void OnClick_Exit()
    {
        Application.Quit();
    }
    public void OnClick_ToggleBoard()
    {
        int size = options.BoardSize;
        size++;
        if(size > 5)
        {
            size = 3;
        }
        options.BoardSize = size;

        SetBoardSelectSprite();
    }
    public void OnClick_ToggleMode()
    {
        GameMode mode = options.mode;
        if(mode == GameMode.LOCAL)
        {
            mode = GameMode.ONLINE;
        }
        else if(mode == GameMode.ONLINE)
        {
            mode = GameMode.BOT;
        }
        else if(mode == GameMode.BOT)
        {
            mode = GameMode.LOCAL;
        }
        options.mode = mode;
        SetModeSprite();
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
    private void SetBoardSelectSprite()
    {
        int size = options.BoardSize;
        Sprite DefaultSprite = Resources.Load<Sprite>($"Sprites/UI/BoardSelectButton_{size}x{size}_Default");
        Sprite SelectedSprite = Resources.Load<Sprite>($"Sprites/UI/BoardSelectButton_{size}x{size}_Selected");
        Sprite PressedSprite = Resources.Load<Sprite>($"Sprites/UI/BoardSelectButton_{size}x{size}_Pressed");
        btnToggleBoard.image.sprite = DefaultSprite;

        SpriteState ss = btnToggleBoard.spriteState;
        ss.highlightedSprite = SelectedSprite;
        ss.pressedSprite = PressedSprite;
        btnToggleBoard.spriteState = ss;
    }
    private void SetModeSprite()
    {
        GameMode mode = options.mode;
        Sprite DefaultSprite = Resources.Load<Sprite>($"Sprites/UI/ModeSelectButton_{mode}_Default");
        Sprite SelectedSprite = Resources.Load<Sprite>($"Sprites/UI/ModeSelectButton_{mode}_Selected");
        Sprite PressedSprite = Resources.Load<Sprite>($"Sprites/UI/ModeSelectButton_{mode}_Pressed");
        btnToggleMode.image.sprite = DefaultSprite;

        SpriteState ss = btnToggleMode.spriteState;
        ss.highlightedSprite = SelectedSprite;
        ss.pressedSprite = PressedSprite;
        btnToggleMode.spriteState = ss;
    }
    public virtual void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(GameScene);
    }

}
