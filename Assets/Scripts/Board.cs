using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Direction { RIGHT, UP, LEFT, DOWN };
public class Board : MonoBehaviour {
    // Board and tile info
    public Tile[,] board;
    public TileState[,] boardState;
    int size = 3;
    float tileSize;
    public Tile tilePrefab;
    public float buildDelay = 0.1f;
    public bool AnimateBoard = true;
    
    // Tokens
    [SerializeField] GameObject p1Token;
    [SerializeField] GameObject p2Token;

    public Texture2D P1Texture { get { return p1Token.GetComponent<SpriteRenderer>().sprite.texture; } }
    public Texture2D P2Texture { get { return p2Token.GetComponent<SpriteRenderer>().sprite.texture; } }
    public Texture2D TileTexture { get { return tilePrefab.GetComponent<SpriteRenderer>().sprite.texture; } }
    

    public int Size { get { return size; } }

    // Directional vectors
    static List<Vector2> directions = new List<Vector2> {
        new Vector2(1,0),
        new Vector2(1,1),
        new Vector2(0,1),
        new Vector2(-1,1),
        new Vector2(-1,0),
        new Vector2(-1,-1),
        new Vector2(0,-1),
        new Vector2(1,-1)
    };

    public void BuildBoard()
    {
        BuildBoard(size);
    }
    /// <summary>
    /// Create and spawn a board of tiles of the given size
    /// </summary>
    /// <param name="size">Side length of the board (3 => 3x3 board)</param>
    public void BuildBoard(int size)
    {
        this.size = size;
        DestroyBoard();
        CalculateTileSize();
        GetComponent<GridLayoutGroup>().constraintCount = size;
        GameManager mgr = FindObjectOfType<GameManager>();

        board = new Tile[size, size];
        boardState = new TileState[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                // Create a new tile at the correct position, and add it to the board matrix
                Tile newTile = Instantiate(tilePrefab, transform);
                
                //newTile.transform.position = startPos + new Vector3(i, j) * tileSize;
                //newTile.transform.localScale *= tileSize;
                board[i, j] = newTile;
                boardState[i, j] = TileState.EMPTY;
                newTile.X = i;
                newTile.Y = j;
                newTile.State = TileState.EMPTY;
                newTile.GetComponent<Button>().onClick.AddListener(delegate { mgr.OnClick_Tile(newTile.X, newTile.Y); });
                // If the board is being animated, hide the tiles as they are made
                if (AnimateBoard)
                    newTile.GetComponent<Image>().enabled = false;
            }
        }
        if (AnimateBoard == true)
            StartCoroutine(AnimateBoardSpiral());
    }
    void CalculateTileSize()
    {
        const float SCALE_FACTOR = 0.5f;
        const float PIXEL_FACTOR = 100f;

        float sqScreenSize = Mathf.Min(Screen.width, Screen.height) * SCALE_FACTOR;
        tileSize = sqScreenSize / size / PIXEL_FACTOR;
    }
    /// <summary>
    /// Animate the board build in a spiral pattern
    /// </summary>
    IEnumerator AnimateBoardSpiral()
    {
        GameManager manager = FindObjectOfType<GameManager>();
        manager.IsInputEnabled = false;

        Direction dir = Direction.RIGHT;
        int x = 0;
        int y = 0;
        bool canMove = true;
        while (canMove)
        {
            board[x, y].GetComponent<Image>().enabled = true;
            yield return new WaitForSeconds(buildDelay);
            // Check if we should change direction
            switch (dir)
            {
                case Direction.RIGHT:
                    if (x + 1 >= Size)
                    {
                        TurnLeft(ref dir);
                    }
                    else if (board[x + 1, y].GetComponent<Image>().enabled == true)
                    {
                        TurnLeft(ref dir);
                    }
                    break;
                case Direction.UP:
                    if (y + 1 >= Size)
                    {
                        TurnLeft(ref dir);
                    }
                    else if (board[x, y + 1].GetComponent<Image>().enabled == true)
                    {
                        TurnLeft(ref dir);
                    }
                    break;
                case Direction.LEFT:
                    if (x - 1 < 0)
                    {
                        TurnLeft(ref dir);
                    }
                    else if (board[x - 1, y].GetComponent<Image>().enabled == true)
                    {
                        TurnLeft(ref dir);
                    }
                    break;
                case Direction.DOWN:
                    if (y - 1 < 0)
                    {
                        TurnLeft(ref dir);
                    }
                    else if (board[x, y - 1].GetComponent<Image>().enabled == true)
                    {
                        TurnLeft(ref dir);
                    }
                    break;
            }

            // Move to the next tile
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
            if (!InBounds(x, y))
            {
                canMove = false;
            }
            // Check if we hit a dead end after turning
            else if(board[x, y].GetComponent<Image>().enabled == true)
            {
                canMove = false;
            }
        }
        manager.IsInputEnabled = true;
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
        GameObject[] tokens = GameObject.FindGameObjectsWithTag("Token");
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        
        // Destroy each token
        for(int i = 0; i < tokens.Length; i++)
        {
            Destroy(tokens[i]);
        }
        // Destroy each tile
        for(int i = 0; i < tiles.Length; i++)
        {
            Destroy(tiles[i]);
        }
        
        // Null the board
        board = null;
        boardState = null;
    }

    public bool MakeMove(Move move)
    {
        if(!IsInBounds(boardState, move.X, move.Y))
        {
            return false;
        }

        Tile targetTile = board[move.X, move.Y];
        // Check if the tile is already taken
        if (targetTile.State == TileState.EMPTY)
        {
            PlaceToken(targetTile, move.player);
            List<Tile> captures = CheckForCaptures(move);
            foreach(Tile t in captures)
            {
                CaptureTile(t, move.player);
            }
            return true;
        }
        else
        {
            if (targetTile.State == PlayerToState(move.player))
            {
                Debug.LogWarning("Cannot place token. Player already owns tile: (" + move.X + "," + move.Y + ")");
                // TODO: Add UI feedback outside debug log
            }
            else
            {
                Debug.LogWarning("Cannot place token. Other player owns tile.");
                // TODO: Add UI feedback outside debug log
            }
            return false;
        }
    }

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
        //token.transform.position += new Vector3(0, 0, -1);
        target.State = PlayerToState(player);
        boardState[target.X, target.Y] = target.State;
    }
    
    public bool CheckWin(out Player winner, out List<Tile> winningTiles)
    {
        winningTiles = new List<Tile>();
        List<int[]> winningCoords;
        bool win = false;
        winner = GetWinner(boardState, out winningCoords);
        if (winner != Player.NONE)
        {
            foreach(int[] c in winningCoords)
            {
                winningTiles.Add(board[c[0], c[1]]);
            }
            win = true;
        }
        return win;        
    }
    
    public bool CheckFull()
    {
        return CheckFull(boardState);
    }

    public List<Tile> CheckForCaptures(Move move)
    {
        List<Tile> captures = new List<Tile>();
        List<int[]> capturesCoords = FindCaptures(boardState, move);
        foreach(int[] c in capturesCoords)
        {
            captures.Add(board[c[0], c[1]]);
        }
        return captures;
    }

    public void CaptureTile(Tile target, Player player)
    {
        PlaceToken(target, player);
    }
    
    private bool InBounds(int x, int y)
    {
        return x >= 0 && x < size && y >= 0 && y < size;
    }
    public void SetTokens(GameObject p1, GameObject p2)
    {
        p1Token = p1;
        p2Token = p2;
    }

    public void HighlightTiles(List<Tile> tiles)
    {
        if(tiles.Count > size)
        {
            Debug.LogWarning("Abnormal Tile Highlight!!");
        }
        foreach (Tile t in board)
        { 
            if (!tiles.Contains(t))
            {
                t.Dim();
            }
            else
            {
                t.ResetAlpha();
            }
        }
    }

    #region Static Methods
    public static TileState PlayerToState(Player player)
    {
        switch(player)
        {
            case Player.P1:
                return TileState.P1;
            case Player.P2:
                return TileState.P2;
            default:
                return TileState.EMPTY;
        }
    }
    public static Player StateToPlayer(TileState state)
    {
        switch (state)
        {
            case TileState.P1:
                return Player.P1;
            case TileState.P2:
                return Player.P2;
            default:
                return Player.NONE;
        }

    }
    public static TileState[,] MakeMove(TileState[,] boardState, Move move)
    {
        TileState[,] newBoard = CloneBoardState(boardState);
        if(newBoard[move.X, move.Y] == TileState.EMPTY)
        {
            newBoard[move.X, move.Y] = PlayerToState(move.player);
            List<int[]> captures = FindCaptures(newBoard, move);
            foreach(int[] c in captures)
            {
                newBoard[c[0], c[1]] = PlayerToState(move.player);
            }
        }
        return newBoard;
    }
    public static Player GetWinner(TileState[,] boardState, out List<int[]> winningCoords)
    {
        int size = boardState.GetLength(0);
        winningCoords = new List<int[]>();
        List<int[]> tileSet = new List<int[]>();

        // Check rows
        for (int r = 0; r < size; r++)
        {
            tileSet.Clear();
            TileState lead = TileState.EMPTY;
            bool win = true;
            for (int c = 0; c < size; c++)
            {
                tileSet.Add(new int[2] { c, r });
                // Winning rows can't have empty tiles
                if (boardState[c, r] == TileState.EMPTY)
                {
                    win = false;
                    break;
                }
                // Get the leader from first tile in row
                if (c == 0)
                {
                    lead = boardState[c, r];
                }
                else if (boardState[c, r] != lead)
                {
                    // No win if tile doesn't match lead
                    win = false;
                    break;
                }
            }
            // Win check for row
            if (win && lead != TileState.EMPTY)
            {
                winningCoords = tileSet;
                return (Player) lead;
            }
        }

        // Check Columns
        for (int c = 0; c < size; c++)
        {
            tileSet.Clear();
            TileState leadState = TileState.EMPTY;
            bool win = true;
            for (int r = 0; r < size; r++)
            {
                tileSet.Add(new int[2] { c, r });
                // Winning columns can't have empty tiles
                if (boardState[c, r] == TileState.EMPTY)
                {
                    win = false;
                    break;
                }
                // Get the leader from the first tile in column
                if (r == 0)
                {
                    leadState = boardState[c, r];
                }
                // No win possible if tile doesn't match the lead
                else if (boardState[c, r] != leadState)
                {
                    win = false;
                    break;
                }
            }
            // Win check for column
            if (win && leadState != TileState.EMPTY)
            {
                winningCoords = tileSet;
                return (Player)leadState;
            }
        }

        // Check first diagonal
        tileSet.Clear();
        TileState d1Lead = TileState.EMPTY;
        bool d1Win = true;
        for (int d = 0; d < size; d++)
        {
            tileSet.Add(new int[2] { d, d });
            // Get the diagonal lead
            if (d == 0)
            {
                d1Lead = boardState[d, d];
                // No win if first tile is empty
                if (d1Lead == TileState.EMPTY)
                {
                    d1Win = false;
                    break;
                }
            }
            // No win if other diagonal tiles don't match
            else if (boardState[d, d] != d1Lead)
            {
                d1Win = false;
                break;
            }
        }
        if (d1Win && d1Lead != TileState.EMPTY)
        {
            winningCoords = tileSet;
            return (Player)d1Lead;
        }

        // Check second diagonal
        tileSet.Clear();
        TileState d2Lead = TileState.EMPTY;
        bool d2Win = true;
        for (int d = 0; d < size; d++)
        {
            tileSet.Add(new int[2] { size - (d + 1), d });
            // Get the diagonal lead
            if (d == 0)
            {
                d2Lead = boardState[size - (d + 1), d];
                // No win if first tile is empty
                if (d2Lead == TileState.EMPTY)
                {
                    d2Win = false;
                    break;
                }
            }
            // No win if other diagonal tiles don't match
            else if (boardState[size - (d + 1), d] != d2Lead)
            {
                d2Win = false;
                break;
            }
        }
        if (d2Win && d2Lead != TileState.EMPTY)
        {
            winningCoords = tileSet;
            return (Player)d2Lead;
        }

        // If the board is full, whoever has the most tiles wins
        if (CheckFull(boardState))
        {
            List<int[]> p1Tiles = new List<int[]>();
            List<int[]> p2Tiles = new List<int[]>();
            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    if (boardState[i, j] == TileState.P1)
                        p1Tiles.Add(new int[2] { i, j });
                    else if (boardState[i, j] == TileState.P2)
                        p2Tiles.Add(new int[2] { i, j });
                }
            }
            int p1Score = p1Tiles.Count;
            int p2Score = p2Tiles.Count;

            if (p1Score + p2Score == size * size)
            {
                if (p1Score > p2Score)
                {
                    winningCoords = p1Tiles;
                    return Player.P1;
                }
                else if (p2Score > p1Score)
                {
                    winningCoords = p2Tiles;
                    return Player.P2;
                }
            }
        }

        // If we get here, there is no winner
        return Player.NONE;
    }
    public static Player GetWinner(TileState[,] boardState)
    {
        List<int[]> winningCoords;
        return GetWinner(boardState, out winningCoords);
    }
    public static bool CheckPlayerWin(TileState[,] boardState, Player player)
    {
        List<int[]> winningCoords;
        return player == GetWinner(boardState, out winningCoords);
    }
    public static bool CheckFull(TileState[,] boardState)
    {
        foreach(TileState t in boardState)
        {
            if(t == TileState.EMPTY)
            {
                return false;
            }
        }
        return true;
    }
    public static bool IsInBounds(TileState[,] boardState, int x, int y)
    {
        return x >= 0 && x < boardState.GetLength(0) && y >= 0 && y < boardState.GetLength(0);
    }
    public static bool IsEdgeTile(TileState[,] boardState, int x, int y)
    {
        int s = boardState.GetLength(0) - 1;
        return x  == 0 || y == 0 || x == s || y == s;
    }
    public static bool IsCornerTile(TileState[,] boardState, int x, int y)
    {
        bool isCorner = false;
        List<int[]> corners = FindCorners(boardState);
        foreach (int[] c in corners)
        {
            if (c[0] == x && c[1] == y && boardState[x, y] == TileState.EMPTY)
            {
                isCorner = true;
            }
        }
        return isCorner;
    }
    public static bool IsVulnerableMove(TileState[,] boardState, Move move)
    {
        bool isVulnerable = false;
        for(int d = 0; d < directions.Count / 2; d++)
        {
            Vector2 dv = directions[d];
            int ax = move.X + (int)dv.x;
            int ay = move.Y + (int)dv.y;
            int ox = move.X + -(int)dv.x;
            int oy = move.Y + -(int)dv.y;
            
            if(IsInBounds(boardState, ax, ay) && IsInBounds(boardState, ox, oy))
            {
                if((boardState[ax, ay] == PlayerToState(move.opponent) && boardState[ox, oy] == TileState.EMPTY) ||
                   (boardState[ox, oy] == PlayerToState(move.opponent) && boardState[ax, ay] == TileState.EMPTY))
                {
                    isVulnerable = true;
                }
            }
        }
        return isVulnerable;
    }
    public static List<int[]> FindCaptures(TileState[,] boardState, Move move)
    {
        List<int[]> captures = new List<int[]>();
        // Check each adjacent tile for the opponent's token
        foreach (Vector2 dir in directions)
        {
            int ax = (int)(move.X + dir.x);
            int ay = (int)(move.Y + dir.y);
            // Bounds check
            if (IsInBounds(boardState, ax, ay))
            {
                //Debug.Log("Adj Tile: (" + adjTile.X + "," + adjTile.Y + ")");
                if (boardState[ax, ay] == PlayerToState(move.opponent))
                {
                    // Check the next tile over
                    int a2x = ax + (int)dir.x;
                    int a2y = ay + (int)dir.y;
                    if (IsInBounds(boardState, a2x, a2y))
                    {
                        // If the next tile over is owned by this player, capture the tile between them
                        if (boardState[a2x, a2y] == PlayerToState(move.player))
                        {
                            captures.Add(new int[2] { ax, ay });
                        }
                    }
                }
            }
        }

        return captures;
    }
    public static List<int[]> FindBlockedCaptures(TileState[,] boardState, Move move)
    {
        List<int[]> blockedCaptures = new List<int[]>();
        // Check each adjacent tile for the opponent's token
        foreach (Vector2 dir in directions)
        {
            int ax = (int)(move.X + dir.x);
            int ay = (int)(move.Y + dir.y);
            // Bounds check
            if (IsInBounds(boardState, ax, ay))
            {
                //Debug.Log("Adj Tile: (" + adjTile.X + "," + adjTile.Y + ")");
                if (boardState[ax, ay] == PlayerToState(move.player))
                {
                    // Check the next tile over
                    int a2x = ax + (int)dir.x;
                    int a2y = ay + (int)dir.y;
                    if (IsInBounds(boardState, a2x, a2y))
                    {
                        // If the next tile over is owned by this player, capture the tile between them
                        if (boardState[a2x, a2y] == PlayerToState(move.opponent))
                        {
                            blockedCaptures.Add(new int[2] { ax, ay });
                        }
                    }
                }
            }
        }

        return blockedCaptures;
    }
    public static List<int[]> FindWinningMoves(TileState[,] boardState, Player player)
    {
        List<int[]> winningMoves = new List<int[]>();
        for(int i = 0; i < boardState.GetLength(0); i++)
        {
            for(int j = 0; j < boardState.GetLength(1); j++)
            {
                Move move = new Move(i, j, player);
                if(boardState[i,j] == TileState.EMPTY)
                {
                    TileState[,] newBoard = MakeMove(boardState, move);
                    if(CheckPlayerWin(newBoard, player))
                    {
                        winningMoves.Add(new int[2] { i, j });
                    }
                }
            }
        }
        return winningMoves;
    }
    public static List<int[]> FindAdjacent(TileState[,] boardState, int x, int y)
    {
        List<int[]> adjacentCoords = new List<int[]>();
        foreach (Vector2 d in directions)
        {
            int ax = x + (int)d.x;
            int ay = y + (int)d.y;
            if(IsInBounds(boardState, ax, ay))
            {
                adjacentCoords.Add(Tile.GetCoordinates(ax, ay));
            }
        }
        return adjacentCoords;
    }
    public static List<int[]> FindAdjacentByState(TileState[,] boardState, int x, int y, TileState state)
    {
        List<int[]> adjCoords = FindAdjacent(boardState, x, y);
        for(int i = adjCoords.Count - 1; i >= 0; i--)
        {
            int[] c = adjCoords[i];
            if(boardState[c[0], c[1]] != state)
            {
                adjCoords.RemoveAt(i);
            }
        }
        return adjCoords;
    }
    public static List<int[]> FindCorners(TileState[,] boardState)
    {
        int s = boardState.GetLength(0) - 1;
        return new List<int[]>()
        {
            new int[2] {0, 0},
            new int[2] {0, s},
            new int[2] {s, 0},
            new int[2] {s, s},
        };
    }
    public static TileState[,] CloneBoardState(TileState[,] boardState)
    {
        TileState[,] clonedBoard = new TileState[boardState.GetLength(0), boardState.GetLength(1)];
        for(int i = 0; i < boardState.GetLength(0); i++)
        {
            for(int j = 0; j < boardState.GetLength(1); j++)
            {
                clonedBoard[i, j] = boardState[i, j];
            }
        }
        return clonedBoard;
    }
    #endregion
}
