using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class MainMenu : Photon.MonoBehaviour, IScreen {
    GameOptions options;
    [SerializeField] Canvas optionsMenu;
    [SerializeField] Canvas introBoard;
    private string GameScene { get { return options.Start3D ? "Game_3D" : "Game"; } }
    [SerializeField]
    Button btnToggleBoard;
    [SerializeField]
    Button btnToggleMode;
    [SerializeField]
    Button btnPlay;

    [SerializeField]
    Image P1TokenSelect;
    [SerializeField]
    Image P2TokenSelect;

    private void Start()
    {
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
        if(introBoard.enabled == true)
        {
            ShowGameOptions();
        }
        else
        {
            StartGame();
        }
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

    public void OnClick_StartTutorial()
    {
        options.BotPrefabName = "TutorialBot";
        options.mode = GameMode.TUTORIAL;
        options.BoardSize = 3;
        StartGame();
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
            options.BotPrefabName = "DefaultBot";
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
        introBoard.enabled = false;
        optionsMenu.enabled = true;
    }

    private void StartGame()
    {
        SetPlayerTokens();
        if (options.IsOnlineGame)
        {
            if (PhotonNetwork.connected == false)
            {
                btnPlay.GetComponent<Animator>().SetBool("Searching", true);
                GetComponent<ConnectAndJoinRandom>().Connect();
            }
            else
            {
                PhotonNetwork.Disconnect();
                btnPlay.GetComponent<Animator>().SetBool("Searching", false);
            }
        }
        else
        {
            SceneManager.LoadScene(GameScene);
        }
    }
    
    private void SetBoardSelectSprite()
    {
        int size = options.BoardSize;
        Sprite DefaultSprite = Resources.Load<Sprite>($"Sprites/UI/BoardSelectButton_{size}x{size}_Default");
        //Sprite SelectedSprite = Resources.Load<Sprite>($"Sprites/UI/BoardSelectButton_{size}x{size}_Selected");
        Sprite PressedSprite = Resources.Load<Sprite>($"Sprites/UI/BoardSelectButton_{size}x{size}_Pressed");
        btnToggleBoard.image.sprite = DefaultSprite;

        SpriteState ss = btnToggleBoard.spriteState;
        //ss.highlightedSprite = SelectedSprite;
        ss.pressedSprite = PressedSprite;
        btnToggleBoard.spriteState = ss;
    }

    private void SetModeSprite()
    {
        // Fix tutorial option if set
        if (options.mode == GameMode.TUTORIAL)
        {
            options.mode = GameMode.BOT;
        }
        GameMode mode = options.mode;
        Sprite DefaultSprite = Resources.Load<Sprite>($"Sprites/UI/ModeSelectButton_{mode}_Default");
        //Sprite SelectedSprite = Resources.Load<Sprite>($"Sprites/UI/ModeSelectButton_{mode}_Selected");
        Sprite PressedSprite = Resources.Load<Sprite>($"Sprites/UI/ModeSelectButton_{mode}_Pressed");
        btnToggleMode.image.sprite = DefaultSprite;

        SpriteState ss = btnToggleMode.spriteState;
        //ss.highlightedSprite = SelectedSprite;
        ss.pressedSprite = PressedSprite;
        btnToggleMode.spriteState = ss;
    }

    private void SetPlayerTokens()
    {
        
        options.P1Token = P1TokenSelect.sprite;
        if (P1TokenSelect.sprite.name == P2TokenSelect.sprite.name)
        {
            options.P2Token = Resources.Load<Sprite>($"Sprites/Tokens/ALT/{P1TokenSelect.sprite.name}_ALT");
        }
        else
        {
            options.P2Token = P2TokenSelect.sprite;
        }
    }
    public virtual void OnJoinedRoom()
    {
        if(PhotonNetwork.isMasterClient)
        {
            StartCoroutine(IE_StartWhenPlayersJoin());
        }
    }

    private IEnumerator IE_StartWhenPlayersJoin()
    {
        while (PhotonNetwork.playerList.Length < 2)
        {
            yield return new WaitForEndOfFrame();
        }
        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("Loading Level");
            PhotonNetwork.LoadLevel(GameScene);
        }
    } 
}
