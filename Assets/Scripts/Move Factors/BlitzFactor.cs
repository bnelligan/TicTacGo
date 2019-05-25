using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlitzFactor : MoveFactor
{
    public BlitzFactor()
    {
        Score = 40;
    }
    public override void CalcFactor(TileState[,] board, ref Move move)
    {
        if (Board.IsMoveBlitz(board, move))
        {
            move.score += Score;
        }
    }
}
