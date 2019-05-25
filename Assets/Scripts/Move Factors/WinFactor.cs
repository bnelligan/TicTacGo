using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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