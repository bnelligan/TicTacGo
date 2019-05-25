using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeFactor : MoveFactor
{
    public EdgeFactor()
    {
        Score = 20;
    }

    public override void CalcFactor(TileState[,] board, ref Move move)
    {
        if (Board.IsEdgeTile(board, move.X, move.Y))
        {
            //Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            move.score += Score;
            //Debug.Log("Move(" + move.X + "," + move.Y + ")" + "score: " + move.score);
        }
    }
}