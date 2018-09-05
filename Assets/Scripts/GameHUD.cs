using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameHUD : MonoBehaviour {

    public GameObject TurnText;

    #region Public Methods
    public void ShowTurn(Player playerTurn)
    {
        SetTurnText(playerTurn.ToString() + " GO!");
    }
    public void ShowTie()
    {
        SetTurnText("TIE GAME!");
    }
    public void ShowWinner(Player winningPlayer)
    {
        SetTurnText(winningPlayer.ToString() + " WINS!");
    }
    #endregion

    private void SetTurnText(string text)
    {
        TurnText.GetComponent<TextMeshProUGUI>().text = text;
    }
}
