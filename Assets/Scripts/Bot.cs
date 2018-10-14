using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour {

    GameManager manager;
    Board board;
    Player botPlayer { get { return manager.Opponent; } }
    bool IsBotTurn { get { return manager.IsBotTurn && manager.IsInputEnabled; } }
    bool MakingMove;

    float MinTurnDelay = 3f;
    float MaxTurnDelay = 5f;    

	// Use this for initialization
	void Start () {
        manager = FindObjectOfType<GameManager>();
        board = manager.GameBoard;
        MakingMove = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (IsBotTurn && !MakingMove)
        {
            MakingMove = true;
            StartCoroutine(MakeMove());
        }
	}
    IEnumerator MakeMove()
    {
        Debug.Log("Thinking...");
        MakingMove = true;
        WaitForSeconds TurnDelay = new WaitForSeconds(Random.Range(MinTurnDelay, MaxTurnDelay));
        yield return TurnDelay;
        List<Move> possibleMoves = GetPossibleMoves();
        for (int m = 0; m < possibleMoves.Count; m++)
        {
            Move move = possibleMoves[m];
            ScoreMove(ref move);
            possibleMoves[m] = move;
            yield return new WaitForEndOfFrame();
        }
        Move bestMove = GetBestMove(possibleMoves);
        Debug.Log("Best move found at (" + bestMove.X + "," + bestMove.Y + " ) score=" + bestMove.score);
        manager.MakeMove(bestMove);
        MakingMove = false;
    }

    List<Move> GetPossibleMoves()
    {
        List<Move> possibleMoves = new List<Move>();
        foreach(Tile tile in board.board)
        {
            if(tile.State == TileState.EMPTY)
            {
                Move possibleMove = new Move(tile, botPlayer);
                possibleMoves.Add(possibleMove);
            }
        }
        return possibleMoves;
    }
    void ScoreMove(ref Move move)
    {
        List<MoveFactor> factors = GetFactorList();
        foreach(MoveFactor f in factors)
        {
            f.CalcFactor(board.boardState, ref move);
        }
    }
    List<MoveFactor> GetFactorList()
    {
        List<MoveFactor> factorList = new List<MoveFactor>();

        factorList.Add(new WinFactor());
        factorList.Add(new CaptureFactor());
        factorList.Add(new BlockLossFactor());
        factorList.Add(new CornerFactor());
        factorList.Add(new EdgeFactor());
        factorList.Add(new AdjacencyFactor());
        factorList.Add(new VulnerabilityFactor());

        return factorList;
    }
    Move GetBestMove(List<Move> moves)
    {
        Move bestMove = new Move();
        bestMove.score = 0;
        foreach(Move m in moves)
        {
            Debug.Log("Move " + m.X + "," + m.Y + " Score=" + m.score);
            if(m.score >= bestMove.score)
            {
                Debug.Log("New best move!");
                bestMove = m;
            }
        }
        return bestMove;
    }
    
}


public abstract class MoveFactor
{
    public int Score = 0;

    public abstract void CalcFactor(TileState[,] board, ref Move move);
}

public class WinFactor : MoveFactor
{
    public WinFactor()
    {
        Score = 500;
    }

    public override void CalcFactor(TileState[,] board, ref Move move)
    {
        TileState[,] newBoard = Board.MakeMove(board, move.X, move.Y, move.player);
        List<int[]> winningTiles;
        if (move.player == Board.GetWinner(newBoard, out winningTiles))
        {
            Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            move.score += Score;
            Debug.Log("New score: " + move.score);
        }

    }
}
public class CaptureFactor : MoveFactor
{
    public CaptureFactor()
    {
        Score = 50;
    }
    public override void CalcFactor(TileState[,] board, ref Move move)
    {
        List<int[]> captures = Board.FindCaptures(board, move.X, move.Y, move.player);
        for (int c = 0; c < captures.Count; c++)
        {
            Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            move.score += Score;
            Debug.Log("New score: " + move.score);
        }
    }
}

public class BlockLossFactor : MoveFactor
{
    public BlockLossFactor()
    {
        Score = 100;
    }
    public override void CalcFactor(TileState[,] board, ref Move move)
    {
        Player opponent = Player.NONE;
        if (move.player == Player.P1)
            opponent = Player.P2;
        else if(move.player == Player.P2)
            opponent = Player.P1;

        TileState[,] newboard = Board.MakeMove(board, move.X, move.Y, opponent);
        if(Board.CheckPlayerWin(newboard, move.opponent))
        {
            Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            move.score += Score;
            Debug.Log("New score: " + move.score);
        }
    }
}
public class CornerFactor : MoveFactor
{
    public CornerFactor()
    {
        Score = 30;
    }
    public override void CalcFactor(TileState[,] board, ref Move move)
    {
        if(Board.IsCornerTile(board, move.X, move.Y))
        {
            Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            move.score += Score;
            Debug.Log("New score: " + move.score);
        }
    }
}
public class EdgeFactor : MoveFactor
{
    public EdgeFactor()
    {
        Score = 15;
    }

    public override void CalcFactor(TileState[,] board, ref Move move)
    { 
        if(Board.IsEdgeTile(board, move.X, move.Y))
        {
            Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            move.score += Score;
            Debug.Log("New score: " + move.score);
        }
    }
}
public class AdjacencyFactor : MoveFactor
{
    int emptyScore = 5;
    int friendlyScore = 15;
    int enemyScore = 10;

    public AdjacencyFactor() { }

    public override void CalcFactor(TileState[,] board, ref Move move)
    {
        List<int[]> adjCoords = Board.FindAdjacent(board, move.X, move.Y);
        foreach (int[] c in adjCoords)
        {
            Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            TileState t = board[c[0], c[1]];
            if (t == Board.PlayerToState(move.player))
            {
                move.score += friendlyScore;
            }
            else if (t == Board.PlayerToState(move.opponent)) 
            {
                move.score += enemyScore;
            }
            else
            {
                move.score += emptyScore;
            }
            Debug.Log("New score: " + move.score);
        }
    }
}
public class VulnerabilityFactor : MoveFactor
{
    public VulnerabilityFactor()
    {
        Score = -50;
    }

    public override void CalcFactor(TileState[,] board, ref Move move)
    {
        if(Board.IsVulnerableMove(board, move))
        {
            Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            move.score += Score;
            Debug.Log("New score: " + move.score);
        }
    }
}
