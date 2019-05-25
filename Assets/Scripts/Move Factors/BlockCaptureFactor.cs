using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCaptureFactor : MoveFactor
{
    public BlockCaptureFactor()
    {
        Score = 50;
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
