using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameHUD : MonoBehaviour {

    public bool ChangeBackColor;
    
    [SerializeField] GameObject BtnRematch;
    [SerializeField] GameObject BtnMenu;
    [SerializeField] TextMeshProUGUI TxtHeader;
    [SerializeField] Image ImgToken1;
    [SerializeField] Image ImgToken2;
    
    public string HeaderText { get { return TxtHeader.text; } set { TxtHeader.text = value; } }

    Camera cam;
    GameManager manager;

    private void Awake()
    {
        cam = Camera.main;
        manager = FindObjectOfType<GameManager>();
        BtnMenu.GetComponent<Button>().onClick.AddListener(manager.OnClick_QuitToMenu);
        BtnRematch.GetComponent<Button>().onClick.AddListener(manager.OnClick_Rematch);
        Reset();
    }
    private void Update()
    {
        BtnRematch.GetComponent<Animator>().SetBool("Searching", manager.RematchFlag);
    }

    #region Public Methods
    public void ShowWaitingForPlayers()
    {
        //HeaderText = "Waiting for players...";
        BtnRematch.SetActive(false);
    }
    public void ShowCurrentTurn()
    {
        Player currentPlayer = manager.ActivePlayer;
        Sprite currentToken = currentPlayer == Player.P1 ? manager.GameBoard.P1Token : manager.GameBoard.P2Token;
        ImgToken1.sprite = currentToken;
        ImgToken2.sprite = currentToken;
        if (manager.IsOnlineGame || manager.IsBotGame)
        {
            if (currentPlayer == manager.LocalPlayer)
            {
                HeaderText = "Your Turn!";
            }
            else
            {
                HeaderText = "Thinking...";
            }
        }
        else
        {
            HeaderText = "Your Turn!";
        }
    }
    public void ShowTie()
    {
        BtnRematch.SetActive(true);
        HeaderText = "Tie Game!";
        ImgToken1.sprite = manager.GameBoard.P1Token;
        ImgToken2.sprite = manager.GameBoard.P2Token;
    }
    public void ShowWinner(Player winningPlayer)
    {
        if (manager.IsOnlineGame || manager.IsBotGame)
        {
            if (manager.LocalPlayer == winningPlayer)
            {
                HeaderText = "YOU WIN!";
            }
            else
            {
                HeaderText = "You Lose.";
            }
        }
        else
        {
            HeaderText = "YOU WIN!";
        }
        BtnRematch.SetActive(true);
        Sprite currentToken = winningPlayer == Player.P1 ? manager.GameBoard.P1Token : manager.GameBoard.P2Token;
        ImgToken1.sprite = currentToken;
        ImgToken2.sprite = currentToken;
        //RematchText = "Rematch";
    }
    public void ShowRequestRematch()
    {
        //HeaderText = "Rematch Requested";
        //RematchText = "Cancel";
    }
    public void ShowCancelRematch()
    {
        //HeaderText = "Rematch Canceled!";
        //RematchText = "Rematch";
    }
    public void Reset()
    {
        //HeaderText = "Starting...";
        BtnRematch.SetActive(false);
    }
    #endregion

    string GetPlayerName(Player player)
    {
        string name = "No Player";
        if(player == Player.P1)
        {
            name = "Player One";
        }
        else if (player == Player.P2)
        {
            name = "Player Two";
        }
        return name;
    }
    
    IEnumerator ShiftBackgroundColor()
    {
        while(ChangeBackColor)
        {

            yield return new WaitForEndOfFrame();
        }
    }
    // Hue change algorithm: https://stackoverflow.com/a/8510751
}
