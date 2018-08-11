/* Brendan Nelligan
 * May 2018
 * Fox Cub Interview
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileState { P1, P2, Empty };
public class Tile : MonoBehaviour {

    public int X;
    public int Y;
    public float dimAlpha = 0.5f;
    public TileState State;

    public void Dim()
    {
        // Dim Tile
        Color tileColor = GetComponent<SpriteRenderer>().color;
        tileColor.a = dimAlpha;
        GetComponent<SpriteRenderer>().color = tileColor;

        // Dim token
        if(transform.childCount > 0)
        {
            Color tokenColor = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
            tokenColor.a = dimAlpha;
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = tokenColor;
            Debug.LogWarning("COLOR CHANGED");
        }
    }
}