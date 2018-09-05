/* Brendan Nelligan
 * May 2018
 * Fox Cub Interview
 */

using System.Collections.Generic;
using UnityEngine;

public enum TileState { P1, P2, Empty };
public class Tile : MonoBehaviour {

    public int X;
    public int Y;
    float originalAlpha;
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
        }
    }

    public static int[,] GetCoordinates(List<Tile> tiles)
    {
        int[,] tileCoords = new int[tiles.Count, 2];
        for (int t = 0; t < tiles.Count; t++)
        {
            tileCoords[t, 0] = tiles[t].X;
            tileCoords[t, 1] = tiles[t].Y;
        }
        return tileCoords;
    }
    public static int[] GetCoordinates(Tile tile)
    {
        int[] coords = new int[2];
        coords[0] = tile.X;
        coords[1] = tile.Y;
        return coords;
    }
}