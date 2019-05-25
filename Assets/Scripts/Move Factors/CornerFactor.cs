using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerFactor : MoveFactor
{
    public CornerFactor()
    {
        Score = 150;
    }
    public override void CalcFactor(TileState[,] board, ref Move move)
    {
        if (Board.IsCornerTile(board, move.X, move.Y))
        {
            //Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            move.score += Score;
            //Debug.Log("Move(" + move.X + "," + move.Y + ")" + "score: " + move.score);
        }
    }
}
