using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;

public enum Player { NONE, P1, P2 };
public enum GameMode { LOCAL, ONLINE,  BOT }
public class GameManager : PunBehaviour {

    public List<Move> MoveHistory; 
    // Private vars
    int turn;


    GameMode mode = GameMode.LOCAL;
    // Config
    public bool RematchFlag = false;
   
    public float moveDelay = 0.1f;
    // Properties
    public bool IsInputEnabled { get; set; }
    public bool IsGameRunning { get; private set; }
    public bool IsOnlineGame { get { return mode == GameMode.ONLINE; } }
    public bool IsLocalGame { get { return mode == GameMode.LOCAL; } }
    public bool IsBotGame { get { return mode == GameMode.BOT; } }
    public bool IsSimulatedGame { get { return IsBotGame && options.IsSimulatedGame; } }
    public Board GameBoard { get { return board; } }
    public int BoardSize { get { return options.BoardSize; } }
    public GameHUD UI { get { return HUD; } }
    public Player ActivePlayer { get; private set; }
    public Player LocalPlayer {
        get {
            if (IsOnlineGame)
                return PhotonNetwork.isMasterClient ? Player.P1 : Player.P2;
            else if (IsBotGame && !options.IsSimulatedGame)
                return Player.P1;
            else if (IsBotGame && options.IsSimulatedGame)
                return Player.NONE;
            else
                return ActivePlayer;
        }
    }
    public Player Opponent {
        get {
            return LocalPlayer == Player.P1 ? Player.P2 : Player.P1;
        }
    }
    public Player Winner { get; private set; }
    public int OnlineWins { get { return PlayerPrefs.GetInt("OnlineWins"); } set { PlayerPrefs.SetInt("OnlineWins", value); } }
    public int OnlineLosses { get { return PlayerPrefs.GetInt("OnlineLosses"); } set { PlayerPrefs.SetInt("OnlineLosses", value); } }
    public int LocalWins { get { return PlayerPrefs.GetInt("LocalWins"); } set { PlayerPrefs.SetInt("LocalWins", value); } }
    public int BotWins { get { return PlayerPrefs.GetInt("BotWins"); } set { PlayerPrefs.SetInt("BotWins", value); } }
    public int BotLosses { get { return PlayerPrefs.GetInt("BotLosses"); } set { PlayerPrefs.SetInt("BotLosses", value); } }
    // External refs
    Board board;
    [SerializeField]
    GameHUD HUD;
    GameOptions options;
    BotManager botManager;
    Bot bot;

    #region Start Logic
    // Use this for initialization
    void Start () {
        Setup();
        if(IsOnlineGame)
        {
            StartCoroutine(IE_StartWhenPlayersJoin());  
        }
        else
        {
            StartGame();
        }
	}
    void Setup()
    {
        if (HUD == null) HUD = GameObject.FindObjectOfType<GameHUD>();
        board = FindObjectOfType<Board>();
        options = FindObjectOfType<GameOptions>();
        if(options == null)
        {
            options = gameObject.AddComponent<GameOptions>();
        }

        MoveHistory = new List<Move>();
        SetGameMode();
        SetFirstPlayer();
    }

	private IEnumerator IE_StartWhenPlayersJoin()
    {
        HUD.ShowWaitingForPlayers();
        while (PhotonNetwork.playerList.Length < 2)
        {
            yield return new WaitForEndOfFrame();
        }
        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("Starting Game");
            photonView.RPC("RPC_StartGame", PhotonTargets.AllBuffered);
        }
    }

    [PunRPC] void RPC_StartGame()
    {
        StartGame();
    }

    public void StartGame()
    {
        if (MoveHistory != null) MoveHistory.Clear();
        else
        {
            MoveHistory = new List<Move>();
        }
        turn = 1;
        IsGameRunning = true;
        IsInputEnabled = true;
        
        if(IsBotGame)
        {
            botManager.StartGame();
        }
        HUD.ShowCurrentTurn();
        board.BuildBoard(BoardSize);
        if (RematchFlag)
        {
            // Loser goes first in rematch
            SwitchTurns(ActivePlayer);
            RematchFlag = false;
        }
    }

    public void StartGame(int size)
    {
        options.BoardSize = size;
        StartGame();
    }
    #endregion
    

    // Update is called once per frame
    void Update () {
        if(IsGameRunning && IsInputEnabled)
        {
            HandleInput();
        }
        if(Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }   
	}
    

    #region Click Events
    public void OnClick_Concede()
    {
        if (IsGameRunning)
        {
            photonView.RPC("RPC_GameOver", PhotonTargets.AllBuffered, Opponent);
        }
        else
        {
            ReturnToMenu();
        }
    }
    public void OnClick_Rematch()
    {
        if (RematchFlag)
        {
            CancelRematch();
        }
        else if(!IsGameRunning)
        {
            RematchFlag = true;
            if(IsOnlineGame)
            {
                HUD.ShowRequestRematch();
                photonView.RPC("RPC_RequestRematch", PhotonTargets.Others);
            }
            else
            {
                Rematch();
            }
        }
    }
    public void OnClick_QuitToMenu()
    {
        if(IsGameRunning)
        {
            if (IsOnlineGame)
                photonView.RPC("RPC_GameOver", PhotonTargets.AllBuffered, Opponent);
            else
                GameOver(Opponent);
        }
        ReturnToMenu();
    }
    public void OnClick_Tile(int x, int y)
    {
        Debug.Log("Clicked tile: (" + x + "," + y + ")");
        if (IsMyTurn(LocalPlayer) && IsGameRunning)
        {
            if (IsOnlineGame)
            {
                Debug.Log("Making Move!");
                photonView.RPC("RPC_MakeMove", PhotonTargets.AllBuffered, x, y, ActivePlayer);
            }
            else
            {
                MakeMove(x, y, ActivePlayer);
            }
        }
    }
    #endregion
    

    #region Private Methods
    private Player RandomPlayer()
    {
        return CoinFlip() ? Player.P1 : Player.P2;
    }
    private bool CoinFlip()
    {
        int i = Random.Range(0, 2);
        return i == 0;
    }
    private void SetFirstPlayer()
    {
        ActivePlayer = Player.P1;
    }
    private void SetGameMode()
    {
        if (PhotonNetwork.connected)
        {
            mode = GameMode.ONLINE;
        }
        else if(options.IsBotGame)
        {
            mode = GameMode.BOT;
            botManager = gameObject.AddComponent<BotManager>();
        }
        else
        {
            mode = GameMode.LOCAL;
        }
        Debug.Log("IsOnline: " + IsOnlineGame.ToString());
        Debug.Log("IsBot: " + IsBotGame.ToString());
        Debug.Log("IsLocal: " + IsLocalGame.ToString());
        Debug.Log("IsSimulated: " + IsSimulatedGame.ToString());
    }
    private void HandleInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit)
            {
                Tile hitTile = hit.collider.GetComponent<Tile>();
                if(hitTile && IsMyTurn(LocalPlayer))
                {
                    if(IsOnlineGame)
                    {
                        Debug.Log("Making Move!");
                        photonView.RPC("RPC_MakeMove", PhotonTargets.AllBuffered, hitTile.X, hitTile.Y, ActivePlayer);
                    }
                    else
                    {
                        MakeMove(hitTile.X, hitTile.Y, ActivePlayer);
                    }
                }
            }
        }
    }
    public bool IsMyTurn(Player player)
    {
        return ActivePlayer == player;
    }
    #endregion


    #region Game Logic
    [PunRPC]
    void RPC_MakeMove(int x, int y, Player player)
    {
        MakeMove(x, y, player);
    }
    public void MakeMove(Tile tile, Player player)
    {
        Move move = new Move(tile.X, tile.Y, player);
        MakeMove(move);
    }
    public void MakeMove(int x, int y, Player player)
    {
        Move move = new Move(x, y, player);
        MakeMove(move);
    }
    public void MakeMove(Move move)
    {
        if (IsGameRunning && ActivePlayer == move.player)
        {
            move.turn = turn;
            if (board.MakeMove(move))
            {
                // Record move
                Debug.Log("Turn: " + move.turn + " | Tile: (" + move.X + "," + move.Y + ") | Player: " + move.player.ToString());
                MoveHistory.Add(move);

                // Check if there is a winner
                Player winner;
                List<Tile> winningTiles;
                if (board.CheckWin(out winner, out winningTiles))
                {
                    int[,] winningTilesCoords = Tile.GetCoordinates(winningTiles);
                    GameOver(winner);
                    board.HighlightTiles(winningTiles);
                }
                else if (board.CheckFull())
                {
                    TieGame();
                }
                else
                {
                    SwitchTurns();
                }
            }
            else
            {
                Debug.Log("Tile already taken.");
            }
        }
    }
    public void MakeDelayedMoves(List<Move> moves)
    {
        StartCoroutine(IE_MakeDelayedMoves(moves));
    }
    IEnumerator IE_MakeDelayedMoves(List<Move> moves)
    {
        IsInputEnabled = false;
        for (int m = 0; m < moves.Count; m++)
        {
            yield return new WaitForSeconds(moveDelay);
            MakeMove(moves[m]);
        }
        IsInputEnabled = true;
    }

    [PunRPC] void RPC_SwitchTurns(Player lastPlayer)
    {
        SwitchTurns(lastPlayer);
    }
    void SwitchTurns()
    {
        turn++;
        // Flip the current player
        if(ActivePlayer == Player.P1)
        {
            ActivePlayer = Player.P2;
        }
        else if(ActivePlayer == Player.P2)
        {
            ActivePlayer = Player.P1;
        }
        // Notify bot manager
        if(IsBotGame)
        {
            botManager.NotifyTurn(ActivePlayer);
        }
        // Update the UI
        HUD.ShowCurrentTurn();
    }
    void SwitchTurns(Player lastPlayer)
    {
        ActivePlayer = lastPlayer;
        SwitchTurns();
    }
    
    [PunRPC] void RPC_GameOver(Player winner, bool tie)
    {
        GameOver(winner, tie);
    }
    void GameOver(Player winner)
    {
        Debug.Log("Winner: " + winner.ToString());
        //SpewCoins();
        //ShowBigWin();
        HUD.ShowWinner(winner);
        IsInputEnabled = false;
        IsGameRunning = false;

        if(IsSimulatedGame)
        {
            botManager.GameOver(winner);
            StartCoroutine(IE_RestartAfterDelay(.1f));
        }

        if(IsLocalGame || winner == LocalPlayer)
        {
            SaveWin();
        }
        else if(winner != LocalPlayer && winner != Player.NONE)
        {
            SaveLoss();
        }
        Debug.Log($"Scores Saved.\n OnlineWins:{OnlineWins} OnlineLosses:{OnlineLosses} LocalWins:{LocalWins} BotWins:{BotWins} BotLosses:{BotLosses}");
        PlayerPrefs.Save();
    }
    void SaveWin()
    {
        // Increase score record
        if(IsOnlineGame)
        {
            OnlineWins++;
        }
        else if(IsLocalGame)
        {
            LocalWins++;
        }
        else if(IsBotGame)
        {
            BotWins++;
        }
    }
    void SaveLoss()
    {
        // Increase score record
        if (IsOnlineGame)
        {
            OnlineLosses++;
        }
        else if (IsBotGame)
        {
            BotLosses++;
        }
    }
    IEnumerator IE_RestartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetGame();
    }
    void TieGame()
    {
        Debug.Log("Tie Game.");
        HUD.ShowTie();
        GameOver(Player.P1, true);
    }
    void GameOver(Player winner, bool tie)
    {
        if(tie)
        {
            TieGame();
        }
        else
        {
            GameOver(winner);
        }
        
    }
    public void EndGame()
    {
        IsInputEnabled = false;
        IsGameRunning = false;
        Winner = Player.NONE;
        board.DestroyBoard();
        HUD.Reset();
       
    }
    public void ResetGame()
    {   
        EndGame();
        StartGame();
    }
    public void ResetGame(int boardSize)
    {
        EndGame();
        StartGame(BoardSize);
    }

    [PunRPC] void RPC_RequestRematch()
    {
        if(RematchFlag)
        {
            photonView.RPC("RPC_Rematch", PhotonTargets.All);
        }
    }
    [PunRPC] void RPC_Rematch()
    {
        Rematch();
    }
    void Rematch()
    {
        RematchFlag = true;
        ResetGame();
    }
    public void CancelRematch()
    {
        RematchFlag = false;
        HUD.ShowCancelRematch();
    }
    public void ReturnToMenu()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }
    #endregion


}

public struct Move
{
    public Move(int x, int y, Player player)
    {
        this.X = x;
        this.Y = y;
        
        this.player = player;
        turn = 0;
        score = 0;
    }
    public int turn;
    public int score;
    public Player player;
    public Player opponent
    {
        get
        {
            if (player == Player.P1)
                return Player.P2;
            else if (player == Player.P2)
                return Player.P1;
            else
                return Player.NONE;
        }
    }
    public int X;
    public int Y;
}


/* FOX CUB
    [SerializeField]
    Coin coinPrefab;
    /// <summary>
    /// Coins to spew on each side of the screen
    /// </summary>
    [SerializeField]
    private int coinsToSpew = 20;
    [SerializeField]
    GameObject bigWinPrefab;

    public void SpewCoins()
    {
        Camera cam = Camera.main;
        for(int c = 0; c < coinsToSpew; c++)
        {
            // Spew coin to the left
            Coin lCoin = Instantiate(coinPrefab);
            lCoin.transform.position = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight / 2, 5)) + new Vector3(2, 0, 0);
            lCoin.LaunchLeft();
            // Spew coin to the right
            Coin rCoin = Instantiate(coinPrefab);
            rCoin.transform.position = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight / 2, 5)) + new Vector3(-2, 0, 0);
            rCoin.LaunchRight();
        }
    }
    public void ShowBigWin()
    {
        Instantiate(bigWinPrefab);
    }
    */
