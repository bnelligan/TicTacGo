using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour {

    GameManager manager;
    Board board;
    public Player BotPlayer = Player.P2;
    public Player OpponentPlayer { get { return BotPlayer == Player.P1 ? Player.P2 : Player.P1; } }
    bool MakingMove;

    [SerializeField]
    float MinTurnDelay = 1f;
    [SerializeField]
    float MaxTurnDelay = 3f;
    public int MutationRange = 10;
    List<MoveFactor> factors;

    // Use this for initialization
    private void Awake()
    {
        factors = new List<MoveFactor>(GetComponents<MoveFactor>());
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
            factors = new List<MoveFactor>(GetComponents<MoveFactor>());
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