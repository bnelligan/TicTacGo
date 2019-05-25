using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureFactor : MoveFactor
{
    public CaptureFactor()
    {
        Score = 75;
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
