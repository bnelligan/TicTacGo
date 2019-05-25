using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LossFactor : MoveFactor
{
    public LossFactor()
    {
        Score = -500;
    }

    public override void CalcFactor(TileState[,] board, ref Move move)
    {
        TileState[,] newBoard = Board.MakeMove(board, move);
        if (Board.CanPlayerWin(newBoard, move.opponent))
        {
            move.score += Score;
        }
    }

}
