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
    public GameObject Token;
    public bool IsTile3D { get { return GetComponent<MeshRenderer>() != null; } }

    public bool IsVisible {
        get
        {
            if(IsTile3D)
            {
                return GetComponent<MeshRenderer>().enabled;
            }
            else
            {
                return GetComponent<Image>().enabled;
            }
        }
        set
        {
            if (IsTile3D)
            {
                GetComponent<MeshRenderer>().enabled = value;
            }
            else
            {
                GetComponent<Image>().enabled = value;
            }
        }
    }

    private void Awake()
    {
        originalAlpha = IsTile3D ? GetComponent<MeshRenderer>().material.color.a : GetComponent<Image>().color.a;
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
        Color tileColor;
        tileColor = IsTile3D ? GetComponent<MeshRenderer>().material.color : GetComponent<Image>().color;

        tileColor.a = a;
        if(IsTile3D)
        {
            GetComponent<MeshRenderer>().material.color = tileColor;
        }
        else
        {
            GetComponent<Image>().color = tileColor;
        }

        // Token
        if (Token != null)
        { 
            Color tokenColor = IsTile3D ? Token.GetComponent<MeshRenderer>().material.color : Token.GetComponent<Image>().color;
            tokenColor.a = a;
            if (IsTile3D)
            {
                Token.GetComponent<MeshRenderer>().material.color = tokenColor;
            }
            else
            {
                Token.GetComponent<Image>().color = tokenColor;
            }
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