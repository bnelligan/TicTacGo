using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveFactor : MonoBehaviour
{
    public int Score = 0;

    public abstract void CalcFactor(TileState[,] board, ref Move move);
    public virtual MoveFactor MutateFactor(int mutationRange)
    {
        MoveFactor factor = (MoveFactor)MemberwiseClone();
        factor.Score += Random.Range(-mutationRange / 2, mutationRange);
        Debug.Log(GetType() + ": " + Score + "=>" + factor.Score);
        return factor;
    }
}
