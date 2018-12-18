using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour {

    GameManager manager;
    Board board;
    public Player BotPlayer = Player.P1;
    public Player OpponentPlayer { get { return BotPlayer == Player.P1 ? Player.P2 : Player.P1; } }
    bool MakingMove;

    float MinTurnDelay = 3f;
    float MaxTurnDelay = 5f;
    public int MutationRange = 10;
    List<MoveFactor> factors;

    // Use this for initialization
    private void Awake()
    {
        factors = NewFactorList();
    }
    void Start () {
        manager = FindObjectOfType<GameManager>();
        board = manager.GameBoard;
        MakingMove = false;
	}
    IEnumerator IE_MakeDelayedMove()
    {
        Debug.Log("Thinking...");
        MakingMove = true;
        WaitForSeconds TurnDelay = new WaitForSeconds(Random.Range(MinTurnDelay, MaxTurnDelay));
        yield return TurnDelay;
        MakeMove();
    }
    public void MakeDelayedMove()
    {
        if(!MakingMove)
        {
            StartCoroutine(IE_MakeDelayedMove());
        }
    }
    public void MakeMove()
    {
        if(!MakingMove)
        {
            MakingMove = true;
        }
        List<Move> possibleMoves = GetPossibleMoves();
        for (int m = 0; m < possibleMoves.Count; m++)
        {
            Move move = possibleMoves[m];
            ScoreMove(ref move);
            possibleMoves[m] = move;
        }
        Move bestMove = GetBestMove(possibleMoves);
        Debug.Log("Best move found at (" + bestMove.X + "," + bestMove.Y + " ) score=" + bestMove.score);
        manager.MakeMove(bestMove);
        MakingMove = false;
        
    }
    public Bot CloneAndMutate()
    {
        Bot clone = gameObject.AddComponent<Bot>();
        clone.SetFactors(GetMutatedFactors());
        return clone;
    }

    List<Move> GetPossibleMoves()
    {
        if (factors == null)
        {
            factors = NewFactorList();
        }
        List<Move> possibleMoves = new List<Move>();
        foreach(Tile tile in board.board)
        {
            if(tile.State == TileState.EMPTY)
            {
                Move possibleMove = new Move(tile.X, tile.Y, BotPlayer);
                possibleMoves.Add(possibleMove);
            }
        }
        return possibleMoves;
    }
    void ScoreMove(ref Move move)
    {
        foreach(MoveFactor f in factors)
        {
            f.CalcFactor(board.boardState, ref move);
        }
    }
    List<MoveFactor> NewFactorList()
    {
        List<MoveFactor> factorList = new List<MoveFactor>
        {
            new WinFactor(),
            new CaptureFactor(),
            new BlockCaptureFactor(),
            new BlockLossFactor(),
            new CornerFactor(),
            new EdgeFactor(),
            new AdjacencyFactor(),
            new VulnerabilityFactor(),
            new OpponentVulnerabilityFactor()
        };

        return factorList;
    }
    List<MoveFactor> GetMutatedFactors()
    {
        List<MoveFactor> mutatedFactors = new List<MoveFactor>();
        Debug.LogWarning("Mutating factors by " + MutationRange + " for bot: " + BotPlayer.ToString());
        foreach(MoveFactor f in factors)
        {
            mutatedFactors.Add(f.MutateFactor(MutationRange));
        }
        return mutatedFactors;
    }
    public void SetFactors(List<MoveFactor> newFactors)
    {
        factors = newFactors;
    }
    
    Move GetBestMove(List<Move> moves)
    {
        List<Move> bestMoves = new List<Move>();
        int bestScore = 0;
        bool firstFlag = true;
        foreach(Move m in moves)
        {
            if (firstFlag)
            {
                bestScore = m.score;
                firstFlag = false;
            }

            //Debug.Log("Move " + m.X + "," + m.Y + " Score=" + m.score);
            if(m.score >= bestScore)
            {
                // Debug.Log("New best move! Score: " + bestScore);
                if(m.score != bestScore)
                {
                    bestMoves.Clear();
                }
                bestMoves.Add(m);
                bestScore = m.score;
            }
        }
        int randIdx = Mathf.FloorToInt(Random.Range(0, bestMoves.Count));
        //Debug.Log("Best Move Index: " + randIdx);
        return bestMoves[randIdx];
    }
    
}


public abstract class MoveFactor
{
    public int Score = 0;

    public abstract void CalcFactor(TileState[,] board, ref Move move);
    public virtual MoveFactor MutateFactor(int mutationRange)
    {
        MoveFactor factor = (MoveFactor)MemberwiseClone();
        factor.Score += Random.Range(-mutationRange/2, mutationRange);
        Debug.Log(GetType() + ": " + Score + "=>" + factor.Score);
        return factor;
    }
}

public class WinFactor : MoveFactor
{
    public WinFactor()
    {
        Score = 1000;
    }

    public override void CalcFactor(TileState[,] board, ref Move move)
    {
        TileState[,] newBoard = Board.MakeMove(board, move);
        List<int[]> winningTiles;
        if (move.player == Board.GetWinner(newBoard, out winningTiles))
        {
            //Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            move.score += Score;
            //Debug.Log("Move(" + move.X + "," + move.Y + ")" + "score: " + move.score);
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
        List<int[]> captures = Board.FindCaptures(board, move);
        for (int c = 0; c < captures.Count; c++)
        {
            //Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            move.score += Score;
            //Debug.Log("Move(" + move.X + "," + move.Y + ")" + "score: " + move.score);
        }
    }
}
public class BlockCaptureFactor : MoveFactor
{
    public BlockCaptureFactor()
    {
        Score = 25;
    }

    public override void CalcFactor(TileState[,] board, ref Move move)
    {
        List<int[]> blockedCaptures = Board.FindBlockedCaptures(board, move);
        for (int c = 0; c < blockedCaptures.Count; c++)
        {
            //Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            move.score += Score;
            //Debug.Log("Move(" + move.X + "," + move.Y + ")" +  "score: " + move.score);
        }
    }
}

public class BlockLossFactor : MoveFactor
{
    public BlockLossFactor()
    {
        Score = 500;
    }
    public override void CalcFactor(TileState[,] board, ref Move move)
    {
        Move oppMove = new Move(move.X, move.Y, move.opponent);
        TileState[,] newboard = Board.MakeMove(board, oppMove);
        if(Board.CheckPlayerWin(newboard, move.opponent))
        {
            //Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            move.score += Score;
            //Debug.Log("Move(" + move.X + "," + move.Y + ")" + "score: " + move.score);
        }
    }
}
public class CornerFactor : MoveFactor
{
    public CornerFactor()
    {
        Score = 100;
    }
    public override void CalcFactor(TileState[,] board, ref Move move)
    {
        if(Board.IsCornerTile(board, move.X, move.Y))
        {
            //Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            move.score += Score;
            //Debug.Log("Move(" + move.X + "," + move.Y + ")" + "score: " + move.score);
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
            //Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            move.score += Score;
            //Debug.Log("Move(" + move.X + "," + move.Y + ")" + "score: " + move.score);
        }
    }
}
public class AdjacencyFactor : MoveFactor
{
    public int emptyScore = 7;
    public int friendlyScore = 15;
    public int enemyScore = 10;

    public AdjacencyFactor() { }

    public override void CalcFactor(TileState[,] board, ref Move move)
    {
        List<int[]> adjCoords = Board.FindAdjacent(board, move.X, move.Y);
        foreach (int[] c in adjCoords)
        {
            //Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
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
            //Debug.Log("Move(" + move.X + "," + move.Y + ")" + "score: " + move.score);
        }
    }
    public override MoveFactor MutateFactor(int mutationRange)
    {
        AdjacencyFactor mutatedFactor = new AdjacencyFactor();
        mutatedFactor.emptyScore = emptyScore + Random.Range(-mutationRange/2, mutationRange);
        Debug.Log("AdjEmpty: " + emptyScore + "=>" + mutatedFactor.emptyScore);
        mutatedFactor.enemyScore = enemyScore + Random.Range(-mutationRange/2, mutationRange);
        Debug.Log("AdjEnemy: " + enemyScore + "=>" + mutatedFactor.enemyScore);
        mutatedFactor.friendlyScore = friendlyScore + Random.Range(-mutationRange/2, mutationRange);
        Debug.Log("AdjFriendly: " + friendlyScore + "=>" + mutatedFactor.friendlyScore);
        return mutatedFactor;
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
            //Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            move.score += Score;
            //Debug.Log("Move(" + move.X + "," + move.Y + ")" + "score: " + move.score);
        }
    }
}
public class OpponentVulnerabilityFactor : MoveFactor
{
    public OpponentVulnerabilityFactor()
    {
        Score = -30;
    }
    public override void CalcFactor(TileState[,] board, ref Move move)
    {
        Move oppMove = new Move(move.X, move.Y, move.opponent);
        if(Board.IsVulnerableMove(board, oppMove))
        {
            //Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            move.score += Score;
            //Debug.Log("Move(" + move.X + "," + move.Y + ")" + "score: " + move.score);
        }
    }
}

