/* Brendan Nelligan
 * 5/31/2018
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class BoardDebugger : Editor {
    
    float tileSize = 50;
    float tileDist = 10;
    int boardSize = 3;
    int sizeIndex = 0;
    int winconIndex = 0;
    int playerIndex = 0;
    Vector2 startPos = new Vector2(10, 30);
    TileState[,] dBoard;
    
    string msgText = "";
    
    // size col wins + size row wins + 2 diagonal wins
    int WinCount { get { return boardSize * 2 + 2; } }
    Player winner {
        get
        {
            if (playerIndex == 0)
                return Player.P1;
            else
                return Player.P2;
        }
    }
    Player loser
    {
        get
        {
            if (playerIndex == 0)
                return Player.P2;
            else
                return Player.P1;
        }
    }

    private void Awake()
    {
        dBoard = new TileState[boardSize, boardSize];
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                dBoard[i, j] = TileState.EMPTY;
                //Debug.Log(dBoard[i, j].ToString());
            }
        }
    }

    public override void OnInspectorGUI()
    {
        GameManager manager = (GameManager)target;
        // Creates a new board if one doesn't exist
        if(dBoard == null)
        {
            dBoard = new TileState[boardSize, boardSize];
            for(int i = 0; i < boardSize; i++)
            {
                for(int j = 0; j < boardSize; j++)
                {
                    dBoard[i, j] = TileState.EMPTY;
                    //Debug.Log(dBoard[i, j].ToString());
                }
            }
        }

        // Win condition selection grid
        GUILayout.Label("Win Condition");
        string[] winOptions = new string[boardSize*2+2];
        for(int i = 0; i < boardSize*2+2; i++)
        {
            if(i < boardSize)
            {
                winOptions[i] = "Col: " + (i + 1);
            }
            else if(i < boardSize*2)
            {
                winOptions[i] = "Row: " + (i - boardSize + 1);
            }
            else
            {
                winOptions[i] = "Diag: " + (i - boardSize*2+1);
            }
            
        }
        winconIndex = GUILayout.SelectionGrid(winconIndex, winOptions, boardSize);

        // Winner select
        GUILayout.Label("Winner");
        playerIndex = GUILayout.SelectionGrid(playerIndex, new string[3] { "Player One", "Player Two", "Tie Game" }, 3);

        // Board Size
        GUILayout.Label("Board Size");
        sizeIndex = GUILayout.SelectionGrid(sizeIndex, new string[2] { "3x3", "4x4" }, 2);
        if (sizeIndex == 0 && boardSize != 3)
            ResizeBoard(3);
        else if (sizeIndex == 1 && boardSize != 4)
            ResizeBoard(4);

        // Test button
        if (GUILayout.Button("Test"))
        {
            if(playerIndex == 2)
            {
                TestTie();
            }
            else
            {
                TestWinCondition();
            }
            
        }

        // Spew coin button
        if (GUILayout.Button("Spew Coins"))
        {
            //manager.SpewCoins();
        }

        // Big Win button
        if(GUILayout.Button("BigWin"))
        {
            //manager.ShowBigWin();
        }
        base.OnInspectorGUI();

        // Message output
        GUILayout.Label(msgText);
    }
    
    /// <summary>
    /// Test out a game with the provided win condition
    /// </summary>
    private void TestWinCondition()
    {
        GameManager manager = (GameManager)target;
        UI_Manager UI = manager.GetComponent<UI_Manager>();
        List<Move> winnerMoves = new List<Move>();
        List<Move> loserMoves = new List<Move>();
        List<Move> moves = new List<Move>();
        Dictionary<string, Tile> freeTiles = new Dictionary<string, Tile>(); // Coord, Tile


        // Restart the game with the new board size
        manager.EndGame();
        manager.GameBoard.AnimateBoard = false;
        manager.StartGame(boardSize);

        // Compile a dictionary of tiles on the board
        foreach (Tile t in manager.GameBoard.board)
        {
            string tkey = t.X + "," + t.Y;
            Debug.Log("Tile added to list with key: " + tkey);
            freeTiles.Add(tkey, t);
        }

        // Find winning moves
        // Column win
        if (winconIndex < boardSize) 
        {
            // Select winning column
            int col = winconIndex;
            for(int r = 0; r < boardSize; r++)
            {
                // Create a move for the current winning tile
                Move tempMove = new Move();
                string mKey = col + "," + r;
                tempMove.tile = freeTiles[mKey];
                tempMove.player = winner;
                winnerMoves.Add(tempMove);
                // Remove tile from free list
                freeTiles.Remove(mKey);
            }
        }
        // Row win
        else if(winconIndex < boardSize*2)
        {
            // Select winning row
            int row  = winconIndex - boardSize;
            for(int c = 0; c < boardSize; c++)
            {
                // Create a move for the current winning tile
                Move tempMove = new Move();
                string mKey = c + "," + row;
                tempMove.tile = freeTiles[mKey];
                tempMove.player = winner;
                winnerMoves.Add(tempMove);
                // Remove tile from free list
                freeTiles.Remove(mKey);
            }
        }
        // Diagonal win
        else
        {
            // Select which diagonal | 0: top left => bottom right | 1: top right => bottom left |
            int diag = winconIndex - boardSize * 2;
            for(int x = 0; x < boardSize; x++)
            {
                int y;
                // Y = X for first diagonal
                if (diag == 0)
                {
                    y = x;
                }
                // Invert Y for the second diagonal
                else
                {
                    y = boardSize - (x + 1);
                }
                // Create a move for the current winning tile
                Move tempMove = new Move();
                string mKey = x + "," + y;
                tempMove.tile = freeTiles[mKey];
                tempMove.player = winner;
                winnerMoves.Add(tempMove);
                // Remove tile from free list
                freeTiles.Remove(mKey);
            }
        }

        // Find losing moves
        Debug.Log("Available tiles:" + freeTiles.Count);
        // Now find out how many moves the loser can make
        int loserMoveCount;
        if(winner == Player.P1) 
        {
            loserMoveCount = boardSize - 1;  // P2 will move one less time whenever p1 wins
        }
        else
        {
            loserMoveCount = boardSize;      // P1 gets to make the same number of moves
        }
        // Convert the remaining tiles into a list for randomization
        List<Tile> loserPossibilities = new List<Tile>(freeTiles.Values);
        while(loserMoveCount > 0 && loserPossibilities.Count > 0)
        {
            // Choose a random tile
            int r = Random.Range(0, loserPossibilities.Count);
            Move tempMove = new Move();
            tempMove.tile = loserPossibilities[r];
            tempMove.player = loser;
            loserPossibilities.RemoveAt(r);
            loserMoves.Add(tempMove);
            loserMoveCount--;
        }
        // Check that the loser can't win on accident.
        if(AreWinningMoves(loserMoves))
        {
            Debug.LogWarning("Loser could win! Replacing move to: (" + loserMoves[0].tile.X + "," + loserMoves[0].tile.Y + ")");
            // Replace the first element with a new random move
            loserMoves.RemoveAt(0);
            int r = Random.Range(0, loserPossibilities.Count);
            Move tempMove = new Move();
            tempMove.tile = loserPossibilities[r];
            tempMove.player = loser;
            loserPossibilities.RemoveAt(r);
            loserMoves.Add(tempMove);
            Debug.LogWarning("Replacement move: (" + tempMove.tile.X + "," + tempMove.tile.Y + ")");
        }
        // Combine move lists by alternating them into a combined stack
        bool winnerTurn = true;
        if (winner == Player.P2)    // P2 goes second 
            winnerTurn = false;
        while (winnerMoves.Count > 0 || loserMoves.Count > 0)
        {
            if (winnerTurn)
            {
                if (winnerMoves.Count > 0)
                {
                    moves.Add(winnerMoves[0]);
                    winnerMoves.RemoveAt(0);
                }
            }
            else
            {
                if (loserMoves.Count > 0)
                {
                    moves.Add(loserMoves[0]);
                    loserMoves.RemoveAt(0);
                }

            }
            // Switch turns
            winnerTurn = !winnerTurn;
        }
            
        manager.MakeDelayedMoves(moves);

        msgText = "Board applied successfully!";
    }
    private bool AreWinningMoves(List<Move> moveList)
    {
        // Cannot be winning moves if there are less than "size" moves
        if(moveList.Count < boardSize)
        {
            return false;
        }
        bool isRow = true;
        bool isCol = true;
        bool isDiag1 = true;
        bool isDiag2 = true;
        Tile lastTile = moveList[0].tile;

        // This check assumes that there will be no more moves than required to win, which is true for my current debugger
        for(int i = 1; i < moveList.Count; i++)
        {
            // Check for column match
            if(moveList[i].tile.X != lastTile.X && isCol == true)
            {
                isCol = false;
            }
            // Check for row match
            if(moveList[i].tile.Y != lastTile.Y && isRow == true)
            {
                isRow = false;
            }
            // Check for first diagonal
            if (moveList[i].tile.X != moveList[i].tile.Y && isDiag1 == true)
            {
                isDiag1 = false;
            }
            // Check for second diagonal
            if (moveList[i].tile.X != boardSize - (moveList[i].tile.Y + 1) && isDiag2 == true)
            {
                isDiag2 = false;
            }
        }

        // If a flag is still true, these are winning moves
        return (isRow || isCol || isDiag1 || isDiag2);
    }
    private void TestTie()
    {
        GameManager manager = (GameManager)target;
        List<Move> moves = new List<Move>();
        List<Move> p1Moves = new List<Move>();
        List<Move> p2Moves = new List<Move>();

        // Restart the game with the new board size
        manager.ResetGame(boardSize);
        Tile[,] board = manager.GameBoard.board;

        // Create a tie board
        Player lead = Player.P1;
        for(int y = 0; y < boardSize; y++)
        {
            int toSwap = 2;
            Player active = lead;
            for(int x = 0; x < boardSize; x++)
            {
                // Swap players every 2 tiles. This creates a gridlock
                if(toSwap == 0)
                {
                    toSwap = 2;
                    if (active == Player.P1)
                        active = Player.P2;
                    else
                        active = Player.P1;
                }
                // Create a move for the active player and tile
                if(active == Player.P1)
                {
                    p1Moves.Add(new Move(board[x, y], active));
                }
                else
                {
                    p2Moves.Add(new Move(board[x, y], active));
                }
                toSwap--;
            }
            // Swap leading player with every new row
            if (lead == Player.P1)
                lead = Player.P2;
            else
                lead = Player.P1;
        }

        // Combine move lists by alternating them into a combined stack
        bool p1Turn = true;
        while (p1Moves.Count > 0 || p2Moves.Count > 0)
        {
            if (p1Turn)
            {
                if (p1Moves.Count > 0)
                {
                    moves.Add(p1Moves[0]);
                    p1Moves.RemoveAt(0);
                }
            }
            else
            {
                if (p2Moves.Count > 0)
                {
                    moves.Add(p2Moves[0]);
                    p2Moves.RemoveAt(0);
                }

            }
            // Switch turns
            p1Turn = !p1Turn;
        }

        manager.MakeDelayedMoves(moves);
        
    }
  
    #region Old Debugger Methods
    private void ToggleTile(int x, int y)
    {
        switch(dBoard[x, y])
        {
            case TileState.EMPTY:
                dBoard[x, y] = TileState.P1;
                break;
            case TileState.P1:
                dBoard[x, y] = TileState.P2;
                break;
            case TileState.P2:
                dBoard[x, y] = TileState.EMPTY;
                break;
        }
    }
    private Texture2D GetTexture(TileState state)
    {
        GameManager manager = (GameManager)target;
        
        switch (state)
        {
            case TileState.P1:
                return manager.GameBoard.P1Texture;
            case TileState.P2:
                return manager.GameBoard.P2Texture;
            default:
                return Resources.Load<Sprite>("Sprites/tile-grid").texture;
        }

    }
    private void ClearBoard()
    {
        dBoard = new TileState[boardSize, boardSize];
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                dBoard[i, j] = TileState.EMPTY;
                //Debug.Log(dBoard[i, j].ToString());
            }
        }
    }
    private void ResizeBoard(int newSize)
    {
        boardSize = newSize;
        ClearBoard();
    }
    private bool ValidateBoard()
    {
        int p1Count = 0;
        int p2Count = 0;

        foreach(TileState tile in dBoard)
        {
            if (tile == TileState.P1)
                p1Count++;
            else if (tile == TileState.P2)
                p2Count++;
        }
        // Moves must be equal, or one player has moved an additional time
        if (p1Count == p2Count || p1Count == p2Count + 1)
        {
            Debug.Log("Valid board |P1:" + p1Count + "|P2:" + p2Count +  "|Turns:" + (p2Count+p1Count));
            return true;
        }
        else
        {
            Debug.LogWarning("Invalid board: |P1:" + p1Count + "|P2:" + p2Count + "|Turns:" + (p2Count + p1Count));
            msgText = "Token count must be equal or p1 +1";
            return false;
        }
    }
    private void ApplyBoard()
    {
        GameManager manager = (GameManager)target;
        UI_Manager UI = manager.GetComponent<UI_Manager>();

        if (ValidateBoard())
        {
            manager.ResetGame(boardSize);

            // Sort the board into two stacks of moves
            List<Move> p1Moves = new List<Move>();
            List<Move> p2Moves = new List<Move>();
            List<Move> moves = new List<Move>();

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (dBoard[j, i] == TileState.P1)
                    {
                        Move move = new Move();
                        move.tile = manager.GameBoard.board[j, boardSize - (i + 1)];
                        move.player = Player.P1;
                        p1Moves.Add(move);
                    }
                    else if (dBoard[j, i] == TileState.P2)
                    {
                        Move move = new Move();
                        move.tile = manager.GameBoard.board[j, boardSize - (i + 1)];
                        move.player = Player.P2;
                        p2Moves.Add(move);
                    }
                }
            }

            // Alternate unstacking moves from each player
            bool p1Turn = true;
            while (p1Moves.Count > 0 || p2Moves.Count > 0)
            {
                if (p1Turn)
                {
                    if (p1Moves.Count > 0)
                    {
                        moves.Add(p1Moves[0]);
                        p1Moves.RemoveAt(0);
                    }

                }
                else
                {
                    if (p2Moves.Count > 0)
                    {
                        moves.Add(p2Moves[0]);
                        p2Moves.RemoveAt(0);
                    }

                }
                // Switch turns
                p1Turn = !p1Turn;
            }
            manager.MakeDelayedMoves(moves);

            msgText = "Board applied successfully!";
        }
        else
        {
            Debug.LogWarning("Cannot apply the board. players must have an equal number of tokens.");
        }


    }
    #endregion
    
}
