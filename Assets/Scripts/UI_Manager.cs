/* Brendan Nelligan
 * May 2018
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameManager))]
public class UI_Manager : MonoBehaviour
{
    // UI Elements - Public
    public Canvas StartCanvas;
    public Canvas GameCanvas;
    public GameObject SizeSelect;       // Radio Buttons
    public GameObject P1Select;    // Scroll rect
    public GameObject P2Select;    // Scroll rect
    public Text MsgText;
    // UI Elements - Private
    private ScrollRect _p1ScrollSelect;
    private ScrollRect _p2ScrollSelect;
    [SerializeField] private GameObject _itemPrefab;

    // Layout properties
    [SerializeField] private float tokenPadding = 10;
    [SerializeField] private bool tokenAutoFit = true;
    // Game Manager
    GameManager manager;
    public Board board
    {
        get { return manager.GameBoard; }
    }

    // Token variables
    Sprite _p1Token;
    Sprite _p2Token;
    public Sprite[] TokenSprites;
    public int p1Index;
    public int p2Index;

    public Sprite P1Token
    {
        get
        {
            return _p1Token;
        }

        private set
        {
            _p1Token = value;
        }
    }
    public Sprite P2Token
    {
        get
        {
            return _p2Token;
        }

        private set
        {
            _p2Token = value;
        }
    }
    private void Awake()
    {
        // Get components
        manager = GetComponent<GameManager>();
        _p1ScrollSelect = P1Select.GetComponent<ScrollRect>();
        _p2ScrollSelect = P2Select.GetComponent<ScrollRect>();
    }
    // Use this for initialization
    void Start()
    { 
        // Start at Main Menu
        GoToMainMenu();
        GameCanvas.enabled = false;

        // Calculate auto size
        float tokenWidth;
        if (tokenAutoFit)
            tokenWidth = P1Select.GetComponent<RectTransform>().rect.width - (tokenPadding * 2);
        else
            tokenWidth = _itemPrefab.GetComponent<RectTransform>().rect.width;

        // Populate token select with tokens
        Vector2 itemPos = Vector2.zero;
        Vector2 dx = new Vector2(0, - (tokenWidth + tokenPadding));
        foreach(Sprite token in TokenSprites)
        {
            itemPos += dx;
            GameObject p1Item = Instantiate(_itemPrefab, itemPos, Quaternion.Euler(0, 0, 0), _p1ScrollSelect.content);
            RectTransform rect = p1Item.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);

            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tokenWidth);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tokenWidth);
            p1Item.GetComponent<Image>().sprite = token;
            
        }
    }

    #region UI Events
    public void OnClick_StartGame()
    {
        // Set the board size from the dropdown
        manager.boardSize = 3;
        // Set the player tokens
        //board.SetTokens(p1Token, p2Token);
        // Start the game 
        manager.GameBoard.AnimateBoard = true;
        manager.StartGame();
        // Enable the correct canvas
        GoToGameUI();
    }
    public void OnClick_Restart()
    {
        manager.GameBoard.AnimateBoard = true;
        manager.ResetGame();
    }
    public void OnClick_Menu()
    {
        manager.EndGame();
        GoToMainMenu();
    }
    public void OnClick_Quit()
    {
        Application.Quit();
    }
    
    #endregion


    #region Public Methods
    /// <summary>
    /// Set the text display showing the current player
    /// </summary>
    /// <param name="player">Current player</param>
    public void SetTurn(Player player)
    {
        if (player == Player.P1)
        {
            MsgText.text = "Player One Go!";
        }
        else if (player == Player.P2)
        {
            MsgText.text = "Player Two Go!";
        }
    }
    public void ShowMessage(string msg)
    {
        MsgText.text = msg;
    }

    public void ShowWinner(Player winner)
    {
        if (winner == Player.P1)
        {
            MsgText.text = "Player One Wins!";
        }
        else if (winner == Player.P2)
        {
            MsgText.text = "Player Two Wins!";
        }

    }
    public void ShowTie()
    {
        MsgText.text = "Tie Game!";
    }

    public void GoToMainMenu()
    {
        StartCanvas.GetComponent<Animator>().SetBool("Open", true);
        GameCanvas.GetComponent<Animator>().SetBool("Open", false);
    }
    public void GoToGameUI()
    {
        GameCanvas.enabled = true;
        StartCanvas.GetComponent<Animator>().SetBool("Open", false);
        GameCanvas.GetComponent<Animator>().SetBool("Open", true);
    }

    public void HighlightTiles(List<Tile> tiles)
    {
        foreach (Tile t in manager.GameBoard.board)
        {
            if (!tiles.Contains(t))
            {
                t.Dim();
            }
        }
    }
    #endregion
}
