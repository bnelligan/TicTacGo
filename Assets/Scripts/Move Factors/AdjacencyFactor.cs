using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjacencyFactor : MoveFactor
{
    public int emptyScore = 7;
    public int friendlyScore = 12;
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
        mutatedFactor.emptyScore = emptyScore + Random.Range(-mutationRange / 2, mutationRange);
        Debug.Log("AdjEmpty: " + emptyScore + "=>" + mutatedFactor.emptyScore);
        mutatedFactor.enemyScore = enemyScore + Random.Range(-mutationRange / 2, mutationRange);
        Debug.Log("AdjEnemy: " + enemyScore + "=>" + mutatedFactor.enemyScore);
        mutatedFactor.friendlyScore = friendlyScore + Random.Range(-mutationRange / 2, mutationRange);
        Debug.Log("AdjFriendly: " + friendlyScore + "=>" + mutatedFactor.friendlyScore);
        return mutatedFactor;
    }
}