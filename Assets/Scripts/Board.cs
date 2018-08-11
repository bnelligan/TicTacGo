/* Brendan Nelligan
 * May 2018
 * Fox Cub Interview
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Direction { RIGHT, UP, LEFT, DOWN };
public class Board : MonoBehaviour {
    // Board and tile info
    public Tile[,] board;
    int _size = 3;
    public float tileSize = 1.3f;
    public Tile tilePrefab;
    public float buildDelay = 0.1f;
    public bool AnimateBoard = true;

    // Tokens
    GameObject p1Token;
    GameObject p2Token;

    public Texture2D P1Texture { get { return p1Token.GetComponent<SpriteRenderer>().sprite.texture; } }
    public Texture2D P2Texture { get { return p2Token.GetComponent<SpriteRenderer>().sprite.texture; } }
    public Texture2D TileTexture { get { return tilePrefab.GetComponent<SpriteRenderer>().sprite.texture; } }
    

    public int Size { get { return _size; } }

    // Directional vectors
    List<Vector2> directions = new List<Vector2> {
<<<<<<< HEAD
        new Vector2(1,0),   
=======
        new Vector2(1,0),
>>>>>>> e175d801c71f37cd8ff7d2de5f5253ceb9e195ae
        new Vector2(1,1),
        new Vector2(0,1),
        new Vector2(-1,1),
        new Vector2(-1,0),
        new Vector2(-1,-1),
        new Vector2(0,-1),
        new Vector2(1,-1)
    };

    /// <summary>
    /// Create and spawn a board of tiles of the given size
    /// </summary>
    /// <param name="size">Side length of the board (3 => 3x3 board)</param>
    public void CreateBoard(int size)
    {
        DestroyBoard();
        _size = size;
        // Start in the top left corner, relative to board center
        Vector3 startPos = transform.position - tileSize * new Vector3((_size - 1) / 2f, (_size - 1) / 2f, 1);
        // Create an empty board matrix of the correct size
        board = new Tile[_size, _size];

        // Spawn tiles and populate the board
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                // Create a new tile at the correct position, and add it to the board matrix
                Tile newTile = Instantiate(tilePrefab, transform);
                newTile.transform.position = startPos + new Vector3(i , j) * tileSize;
                board[i, j] = newTile;
                newTile.X = i;
                newTile.Y = j;
                newTile.State = TileState.Empty;
                // If the board is being animated, hide the tiles as they are made
                if(AnimateBoard)
                    newTile.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        if (AnimateBoard == true)
            StartCoroutine(BuildBoardSpiral());
    }

    /// <summary>
    /// Build the board in a spiral pattern
    /// </summary>
    public IEnumerator BuildBoardSpiral()
    public IEnumerator BuildBoard()
    {
        GameManager manager = FindObjectOfType<GameManager>();
        manager.UI.ShowMessage("Building board...");
        manager.InputEnabled = false;

        Direction dir = Direction.RIGHT;
        int x = 0;
        int y = 0;
        bool canMove = true;
        while (canMove)
        {
            board[x, y].GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(buildDelay);
            // Check if we should change direction
            switch (dir)
            {
                case Direction.RIGHT:
                    if (x + 1 >= Size)
                    {
                        TurnLeft(ref dir);
                    }
                    else if (board[x + 1, y].GetComponent<SpriteRenderer>().enabled == true)
                    {
                        TurnLeft(ref dir);
                    }
                    break;
                case Direction.UP:
                    if (y + 1 >= Size)
                    {
                        TurnLeft(ref dir);
                    }
                    else if (board[x, y + 1].GetComponent<SpriteRenderer>().enabled == true)
                    {
                        TurnLeft(ref dir);
                    }
                    break;
                case Direction.LEFT:
                    if (x - 1 < 0)
                    {
                        TurnLeft(ref dir);
                    }
                    else if (board[x - 1, y].GetComponent<SpriteRenderer>().enabled == true)
                    {
                        TurnLeft(ref dir);
                    }
                    break;
                case Direction.DOWN:
                    if (y - 1 < 0)
                    {
                        TurnLeft(ref dir);
                    }
                    else if (board[x, y - 1].GetComponent<SpriteRenderer>().enabled == true)
                    {
                        TurnLeft(ref dir);
                    }
                    break;
            }

            // Move in the direction
            switch (dir)
            {
                case Direction.RIGHT:
                    x++;
                    break;
                case Direction.UP:
                    y++;
                    break;
                case Direction.LEFT:
                    x--;
                    break;
                case Direction.DOWN:
                    y--;
                    break;
            }
            // Bounds check on move
            if (x < 0 || y < 0 || x >= Size || y >= Size)
            {
                canMove = false;
            }
            // Check if we hit a dead end after turning
            else if(board[x, y].GetComponent<SpriteRenderer>().enabled == true)
            {
                canMove = false;
            }
        }
        manager.InputEnabled = true;
        manager.UI.SetTurnText(Player.P1);
    }

    void TurnLeft(ref Direction start)
    {
        switch(start)
        {
            case Direction.DOWN:
                start = Direction.RIGHT;
                break;
            case Direction.RIGHT:
                start = Direction.UP;
                break;
            case Direction.UP:
                start = Direction.LEFT;
                break;
            case Direction.LEFT:
                start = Direction.DOWN;
                break;
        }
            
    }

    public void DestroyBoard()
    {
        // Find all tiles and tokens
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        GameObject[] tokens = GameObject.FindGameObjectsWithTag("Token");

        // Destroy each tile
        for(int til = 0; til < tiles.Length; til++)
        {
            Destroy(tiles[til]);
        }
        // Destroy each token
        for(int tok = 0; tok < tokens.Length; tok++)
        {
            Destroy(tokens[tok]);
        }
        // Null the board
        board = null;
    }

    public bool MakeMove(Tile target, Player player)
    {
        // Check if the tile is already taken
        if (target.State != TileState.Empty)
        {
            // Enums are designed to be compared
            if ((byte)target.State == (byte)player)
            {
                Debug.LogWarning("Cannot place token. Player already owns tile: (" + target.X + "," + target.Y + ")");
                // TODO: Add UI feedback outside debug log
            }
            else
            {
                Debug.LogWarning("Cannot place token. Other player owns tile.");
                // TODO: Add UI feedback outside debug log
            }
            return false;
        }
        // Tile is free!
        else
        {
            PlaceToken(target, player);
            CheckCapture(target, player);
            return true;
        }
    }

    /// <summary>
    /// Places or replaces a token at the target tile
    /// </summary>
    /// <param name="target"></param>
    /// <param name="player"></param>
    private void PlaceToken(Tile target, Player player)
    {
        // Destroy any tokens on that tile
        for(int c = 0; c < target.transform.childCount; c++)
        {
            Transform currentToken = target.transform.GetChild(c);
            if(currentToken.CompareTag("Token"))
                Destroy(currentToken.gameObject);
        }
        // Place the player token on the tile
        GameObject token;
        if (player == Player.P1)
            token = p1Token;
        else
            token = p2Token;
        token = Instantiate(token, target.transform);
        token.transform.position += new Vector3(0, 0, -1);
        target.State = (TileState)player;
    }
    
    /// <summary>
    /// Check if a player has won
    /// </summary>
    /// <param name="winner">Output variable of the winner</param>
    /// <returns>True if there is a winner, and an output of which player won</returns>
    public bool CheckWin(out Player winner, out List<Tile> winningTiles)
    {
        // Winner must be set because it is output. It is not used if this returns false
        winner = Player.P1;
        List<Tile> tileSet = new List<Tile>();
        winningTiles = tileSet;
        // Check rows
        for(int r = 0; r < _size; r++)
        {
            tileSet.Clear();
            TileState lead = TileState.Empty;
            bool win = true;
            for(int c = 0; c < _size; c++)
            {
                tileSet.Add(board[c, r]);
                // Winning rows can't have empty tiles
                if (board[c,r].State == TileState.Empty)
                {
                    win = false;
                    break;
                }
                // Get the leader from first tile in row
                if(c == 0)
                {
                    lead = board[c,r].State;
                }
                else if (board[c, r].State != lead)
                {
                    // No win if tile doesn't match lead
                    win = false;
                    break;
                }
            }
            // Win check for row
            if(win && lead != TileState.Empty)
            {
                winner = (Player)lead;
                winningTiles = tileSet;
                return true;
            }
        }

        // Check Columns
        for (int c = 0; c < _size; c++)
        {
            tileSet.Clear();
            TileState lead = TileState.Empty;
            bool win = true;
            for (int r = 0; r < _size; r++)
            {
                tileSet.Add(board[c, r]);
                // Winning columns can't have empty tiles
                if (board[c, r].State == TileState.Empty)
                {
                    win = false;
                    break;
                }
                // Get the leader from the first tile in column
                if (r == 0)
                {
                    lead = board[c, r].State;
                }
                // No win possible if tile doesn't match the lead
                else if (board[c, r].State != lead)
                {
                    win = false;
                    break;
                }
            }
            // Win check for column
            if (win && lead != TileState.Empty)
            {
                winner = (Player)lead;
                winningTiles = tileSet;
                return true;
            }
        }

        // Check first diagonal
        TileState d1Lead = TileState.Empty;
        bool d1Win = true;
        tileSet.Clear();
        for(int d = 0; d < _size; d++)
        {
            tileSet.Add(board[d, d]);
            // Get the diagonal lead
            if (d == 0)
            {
                d1Lead = board[d, d].State;
                // No win if first tile is empty
                if(d1Lead == TileState.Empty)
                {
                    d1Win = false;
                    break;
                }
            }
            // No win if other diagonal tiles don't match
            else if (board[d,d].State != d1Lead)
            {
                d1Win = false;
                break;
            }
        }
        if(d1Win && d1Lead != TileState.Empty)
        {
            winner = (Player)d1Lead;
            winningTiles = tileSet;
            return true;
        }

        // Check second diagonal
        TileState d2Lead = TileState.Empty;
        bool d2Win = true;
        tileSet.Clear();
        for (int d = 0; d < _size; d++)
        {
            tileSet.Add(board[_size - (d + 1), d]);
            // Get the diagonal lead
            if (d == 0)
            {
                d2Lead = board[_size - (d + 1), d].State;
                // No win if first tile is empty
                if (d2Lead == TileState.Empty)
                {
                    d2Win = false;
                    break;
                }
            }
            // No win if other diagonal tiles don't match
            else if (board[_size - (d + 1), d].State != d2Lead)
            {
                d2Win = false;
                break;
            }
        }
        if (d2Win && d2Lead != TileState.Empty)
        {
            winner = (Player)d2Lead;
            winningTiles = tileSet;
            return true;
        }

        // If we get here, there is no winner
        return false;
    }
    

    /// <summary>
    /// Check if the board is full
    /// </summary>
    /// <returns></returns>
    public bool CheckFull()
    {
        bool full = true;
        foreach(Tile t in board)
        {
            if(t.State == TileState.Empty)
            {
                full = false;
            }
        }
        return full;
    }

    public void CheckCapture(Tile target, Player player)
    {
        // Set the opponent
        Player opponent;
        if (player == Player.P1)
            opponent = Player.P2;
        else
            opponent = Player.P1;
        //Debug.Log("Checking capture on tile: (" + target.X + "," + target.Y + ")");
        // Check each adjacent tile for the opponent's token
        foreach(Vector2 dir in directions)
        {
            Tile adjTile;
            int ax = (int)(target.X + dir.x);
            int ay = (int)(target.Y + dir.y);
            // Bounds check
            if(ax >= 0 && ax < _size && ay >= 0 && ay < _size)
            {
                adjTile = board[ax, ay];
                //Debug.Log("Adj Tile: (" + adjTile.X + "," + adjTile.Y + ")");
                if (adjTile.State == (TileState)opponent)
                {
                    // Check the next tile over
                    ax += (int)dir.x;
                    ay += (int)dir.y;
                    if (ax >= 0 && ax < _size && ay >= 0 && ay < _size)
                    {
                        // If the next tile over is owned by this player, capture the tile between them
                        Tile nextTile = board[ax, ay];
                        if (nextTile.State == (TileState)player)
                        {
                            PlaceToken(adjTile, player);
                        }
                    }
                }
            }
            else
            {
                
            }
        }

    }
    
    public void SetTokens(GameObject p1, GameObject p2)
    {
        p1Token = p1;
        p2Token = p2;
    }
}
