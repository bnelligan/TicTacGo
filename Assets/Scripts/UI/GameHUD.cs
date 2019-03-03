using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameHUD : MonoBehaviour {
    public bool ChangeBackColor;

    #region HUD Elements
    [SerializeField] GameObject BtnRematch;
    [SerializeField] GameObject BtnMenu;
    #endregion

    #region Element Accessors
    #endregion

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

    #region Public Methods
    public void ShowWaitingForPlayers()
    {
        //HeaderText = "Waiting for players...";
        BtnRematch.SetActive(false);
    }
    public void ShowCurrentTurn()
    {
        //if (manager.IsLocalGame)
        //{
        //    TurnText = GetPlayerName(manager.ActivePlayer) + " Go!";
        //}
        //else if (manager.IsSimulatedGame)
        //{
        //    if (manager.IsMyTurn(Player.P1))
        //    {
        //        TurnText = "Bot1 Turn!";
        //    }
        //    else
        //    {
        //        TurnText = "Bot2 Turn!";
        //    }
        //}
        //else if (manager.IsBotGame || manager.IsOnlineGame)
        //{
        //    if (manager.IsMyTurn(manager.LocalPlayer))
        //    {
        //        TurnText = "Your Turn!";
        //    }
        //    else
        //    {
        //        TurnText = "Opponent Turn...";
        //    }
        //}
    }
    public void ShowTie()
    {
        //TurnText = "Tie Game!";
        // BtnRematch.SetActive(true);
    }
    public void ShowWinner(Player winningPlayer)
    {
        //if(manager.IsOnlineGame || manager.IsBotGame)
        //{
        //    if(manager.LocalPlayer == winningPlayer)
        //    {
        //        HeaderText = "YOU WIN!";
        //    }
        //    else
        //    {
        //        HeaderText = "You Lost";
        //    }
        //}
        //else
        //{
        //    HeaderText = winningPlayer.ToString() + " WINS!";
        //}
        BtnRematch.SetActive(true);
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
