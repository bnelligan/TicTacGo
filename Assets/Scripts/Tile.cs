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

    [SerializeField]
    private bool glowing = false;
    private bool dimmed = false;
    private float gAmp = 0.15f;
    private float gPeriod = 6;
    private float gStart = 0f;
    private float bRatio = 0.1843f;
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
        Material oldMat = IsTile3D ? GetComponent<MeshRenderer>().material : GetComponent<Image>().material;
        Material newMat = new Material(oldMat);
        if (IsTile3D)
        {
            GetComponent<MeshRenderer>().material = newMat;
        }
        else
        {
            GetComponent<Image>().material = newMat;
        }
        originalAlpha = IsTile3D ? GetComponent<MeshRenderer>().material.color.a : GetComponent<Image>().material.color.a;
        //StartGlow();
    }
    private void Update()
    {
        UpdateGlow();
    }
    private void UpdateGlow()
    {
        Color tileColor = IsTile3D ? GetComponent<MeshRenderer>().material.color : GetComponent<Image>().color;
        if (glowing)
        {
            // Glow with a sin curve towards a blue-green hue
            float t = Time.time - gStart;
            tileColor.r = 1 - (gAmp * Mathf.Sin(t * gPeriod) + gAmp);
            tileColor.b = 1 - (gAmp * bRatio * Mathf.Sin(t * gPeriod) + gAmp * bRatio);
            tileColor.g = 1;
            tileColor.a = GetComponent<Image>().material.color.a;
            if (IsTile3D)
            {
                GetComponent<MeshRenderer>().material.color = tileColor;
            }
            else
            {
                GetComponent<Image>().material.color = tileColor;
            }
        }
        else
        {
            // Reset glow
            tileColor.r = 1;
            tileColor.b = 1;
            tileColor.g = 1;
            tileColor.a = GetComponent<Image>().material.color.a;
            if (IsTile3D)
            {
                GetComponent<MeshRenderer>().material.color = tileColor;
            }
            else
            {
                GetComponent<Image>().material.color = tileColor;
            }
        }
        // Dimming
        if(dimmed)
        {
            tileColor.a = dimAlpha * originalAlpha;
            // Token
            if (Token != null)
            {
                Color tokenColor = IsTile3D ? Token.GetComponent<MeshRenderer>().material.color : Token.GetComponent<Image>().material.color;
                tokenColor.a = dimAlpha * originalAlpha;
                if (IsTile3D)
                {
                    Token.GetComponent<MeshRenderer>().material.color = tokenColor;
                }
                else
                {
                    Token.GetComponent<Image>().material.color = tokenColor;
                }
            }
        }
        else
        {
            tileColor.a = originalAlpha;
            // Token
            if (Token != null)
            {
                Color tokenColor = IsTile3D ? Token.GetComponent<MeshRenderer>().material.color : Token.GetComponent<Image>().material.color;
                tokenColor.a = originalAlpha;
                if (IsTile3D)
                {
                    Token.GetComponent<MeshRenderer>().material.color = tokenColor;
                }
                else
                {
                    Token.GetComponent<Image>().material.color = tokenColor;
                }
            }
        }
        // Set color
        if (IsTile3D)
        {
            GetComponent<MeshRenderer>().material.color = tileColor;
        }
        else
        {
            GetComponent<Image>().material.color = tileColor;
        }
    }
    public void Dim()
    {
        dimmed = true;
    }
    public void ResetAlpha()
    {
        dimmed = false;
    }
    public void SetAlpha(float a)
    {
        // Tile
        Color tileColor;
        tileColor = IsTile3D ? GetComponent<MeshRenderer>().material.color : GetComponent<Image>().material.color;

        tileColor.a = a;
        if(IsTile3D)
        {
            GetComponent<MeshRenderer>().material.color = tileColor;
        }
        else
        {
            GetComponent<Image>().material.color = tileColor;
        }

        
    }

    public void StartGlow()
    {
        gStart = Time.time;
        glowing = true;
    }
    public void StopGlow()
    {
        glowing = false;
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