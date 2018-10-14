/* Brendan Nelligan
 * May 2018
 * Fox Cub Interview
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TileState {EMPTY, P1, P2 };
public class Tile : MonoBehaviour {

    public int X;
    public int Y;
    public TileState State;
    float originalAlpha;
    float dimAlpha = 0.5f;
    
    private void Start()
    {
        originalAlpha = GetComponent<Image>().color.a;
    }

    public void Dim()
    {
        SetAlpha(dimAlpha*originalAlpha);
    }
    public void ResetAlpha()
    {
        SetAlpha(originalAlpha);
    }
    public void SetAlpha(float a)
    {
        // Tile
        Color tileColor = GetComponent<Image>().color;
        tileColor.a = a;
        GetComponent<Image>().color = tileColor;

        // Token
        if (transform.childCount > 0)
        {
            Color tokenColor = transform.GetChild(0).GetComponent<Image>().color;
            tokenColor.a = a;
            transform.GetChild(0).GetComponent<Image>().color = tokenColor;
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
    public static int[] GetCoordinates(int x, int y)
    {
        return new int[2] { x, y };
    }
}