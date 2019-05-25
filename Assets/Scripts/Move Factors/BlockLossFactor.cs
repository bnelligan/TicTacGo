using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockLossFactor : MoveFactor
{
    public BlockLossFactor()
    {
        Score = 750;
    }
    public override void CalcFactor(TileState[,] board, ref Move move)
    {
        Move oppMove = new Move(move.X, move.Y, move.opponent);
        TileState[,] newboard = Board.MakeMove(board, oppMove);
        if (Board.CheckPlayerWin(newboard, move.opponent))
        {
            //Debug.Log("Adding " + GetType().ToString() + " to move: " + move);
            move.score += Score;
            //Debug.Log("Move(" + move.X + "," + move.Y + ")" + "score: " + move.score);
        }
    }
}