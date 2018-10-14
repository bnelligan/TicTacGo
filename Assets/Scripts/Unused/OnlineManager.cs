using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// the Photon server assigns a ActorNumber (player.ID) to each player, beginning at 1
// for this game, we don't mind the actual number
// this game uses player 0 and 1, so clients need to figure out their number somehow

[RequireComponent(typeof(GameManager))]


public class OnlineManager : PunBehaviour, IPunTurnManagerCallbacks
{
    // Game Manager
    GameManager manager;

    public Board board
    {
        get { return manager.GameBoard; }
    }

    // Token variables
    [SerializeField]
    private RectTransform ConnectUiView;

    [SerializeField]
    private RectTransform GameUiView;

    [SerializeField]
    private CanvasGroup ButtonCanvasGroup;

    [SerializeField]
    private RectTransform TimerFillImage;

    [SerializeField]
    private Text TurnText;

    [SerializeField]
    private Text TimeText;

    [SerializeField]
    private Text RemotePlayerText;

    [SerializeField]
    private Text LocalPlayerText;

    [SerializeField]
    private Image WinOrLossImage;

    [SerializeField]
    private Sprite SpriteWin;

    [SerializeField]
    private Sprite SpriteLose;

    [SerializeField]
    private Sprite SpriteDraw;

    [SerializeField]
    private RectTransform DisconnectedPanel;

    private ResultType result;

    private PunTurnManager turnManager;

    // keep track of when we show the results to handle game logic.
    private bool IsShowingResults;

    public enum ResultType
    {
        None = 0,
        Draw,
        LocalWin,
        LocalLoss
    }

    public void Update()
    {

        if (!PhotonNetwork.inRoom)
        {
            return;
        }

        // disable the "reconnect panel" if PUN is connected or connecting
        if (PhotonNetwork.connected && this.DisconnectedPanel.gameObject.GetActive())
        {
            this.DisconnectedPanel.gameObject.SetActive(false);
        }
        if (!PhotonNetwork.connected && !PhotonNetwork.connecting && !this.DisconnectedPanel.gameObject.GetActive())
        {
            this.DisconnectedPanel.gameObject.SetActive(true);
        }


        if (PhotonNetwork.room.PlayerCount > 1)
        {
            if (this.turnManager.IsOver)
            {
                return;
            }

            if (this.TurnText != null)
            {
                this.TurnText.text = this.turnManager.Turn.ToString();
            }

            if (this.turnManager.Turn > 0 && this.TimeText != null && !IsShowingResults)
            {

                this.TimeText.text = this.turnManager.RemainingSecondsInTurn.ToString("F1") + " SECONDS";

                TimerFillImage.anchorMax = new Vector2(1f - this.turnManager.RemainingSecondsInTurn / this.turnManager.TurnDuration, 1f);
            }
        }
        else
        {
            ButtonCanvasGroup.interactable = PhotonNetwork.room.PlayerCount > 1;

            if (PhotonNetwork.room.PlayerCount < 2)
            {
                //
            }

            // if the turn is not completed by all, we use a random image for the remote hand
            else if (this.turnManager.Turn > 0 && !this.turnManager.IsCompletedByAll)
            {
                // alpha of the remote hand is used as indicator if the remote player "is active" and "made a turn"
                PhotonPlayer remote = PhotonNetwork.player.GetNext();

                float alpha = 0.5f;

                if (this.turnManager.GetPlayerFinishedTurn(remote))
                {
                    alpha = 1;
                }
                if (remote != null && remote.IsInactive)
                {
                    alpha = 0.1f;
                }
            }
        }

    }

    /// <summary>Called when a turn begins (Master Client set a new Turn number).</summary>
    public void OnTurnBegins(int turn)
    {
        Debug.Log("OnTurnBegins() turn: " + turn);
    }

    public void OnTurnCompleted(int obj)
    {
        Debug.Log("OnTurnCompleted: " + obj);
        this.UpdateScores();
        this.OnEndTurn();
    }

    // when a player moved (but did not finish the turn)
    public void OnPlayerMove(PhotonPlayer photonPlayer, int turn, object move)
    {
        Debug.Log("OnPlayerMove: " + photonPlayer + " turn: " + turn + " action: " + move);
        throw new NotImplementedException();
    }

    public void OnTurnTimeEnds(int obj)
    {
        if (!IsShowingResults)
        {
            Debug.Log("OnTurnTimeEnds: Calling OnTurnCompleted");
            OnTurnCompleted(-1);
        }
    }

    private void UpdateScores()
    {
        if (this.result == ResultType.LocalWin)
        {
            PhotonNetwork.player.AddScore(1);   // this is an extension method for PhotonPlayer. you can see it's implementation
        }
    }

    /// <summary>Call to start the turn (only the Master Client will send this).</summary>
    public void StartTurn()
    {
        if (PhotonNetwork.isMasterClient)
        {
            this.turnManager.BeginTurn();
        }
    }

    public void OnEndTurn()
    {
        this.StartCoroutine("ShowResultsBeginNextTurnCoroutine");
    }

    public void EndGame()
    {
        Debug.Log("EndGame");
    }

    private void UpdatePlayerTexts()
    {
        PhotonPlayer remote = PhotonNetwork.player.GetNext();
        PhotonPlayer local = PhotonNetwork.player;

        if (remote != null)
        {
            // should be this format: "name        00"
            this.RemotePlayerText.text = remote.NickName + "        " + remote.GetScore().ToString("D2");
        }
        else
        {

            TimerFillImage.anchorMax = new Vector2(0f, 1f);
            this.TimeText.text = "";
            this.RemotePlayerText.text = "waiting for another player        00";
        }

        if (local != null)
        {
            // should be this format: "YOU   00"
            this.LocalPlayerText.text = "YOU   " + local.GetScore().ToString("D2");
        }
    }

    public void OnClickConnect()
    {
        PhotonNetwork.ConnectUsingSettings(null);
        PhotonHandler.StopFallbackSendAckThread();  // this is used in the demo to timeout in background!
    }

    public void OnClickReConnectAndRejoin()
    {
        PhotonNetwork.ReconnectAndRejoin();
        PhotonHandler.StopFallbackSendAckThread();  // this is used in the demo to timeout in background!
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom()");
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.room.PlayerCount == 2)
        {
            if (this.turnManager.Turn == 0)
            {
                this.StartTurn();
            }
        }
        else
        {
            Debug.Log("Waiting for another player");
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("Other player arrived");

        if (PhotonNetwork.room.PlayerCount == 2)
        {
            if (this.turnManager.Turn == 0)
            {
                // when the room has two players, start the first turn (later on, joining players won't trigger a turn)
                this.StartTurn();
            }
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log("Other player disconnected! " + otherPlayer.ToStringFull());
    }

    public override void OnConnectionFail(DisconnectCause cause)
    {
        this.DisconnectedPanel.gameObject.SetActive(true);
    }

    public void OnClick_StartGame()
    {
        // Set the board size from the dropdown
        // Set the player tokens
        //board.SetTokens(p1Token, p2Token);
        // Start the game 
        manager.GameBoard.AnimateBoard = true;
        manager.StartGame();
        // Enable the correct canvas
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

    public void OnPlayerFinished(PhotonPlayer player, int turn, object move)
    {
        throw new NotImplementedException();
    }

}