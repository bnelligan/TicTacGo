using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snapheader : MonoBehaviour
{
    public Vector2 landscapePosition;
    public Vector2 portraitPosition;
    bool portraitMode { get { return Screen.height > Screen.width; } }
    private void Awake()
    {
        SnapPosition();
    }
    private void Update()
    {
        SnapPosition();
    }
    void SnapPosition()
    {
        if (portraitMode)
        {
            GetComponent<RectTransform>().anchoredPosition = portraitPosition;

        }
        else
        {
            GetComponent<RectTransform>().anchoredPosition = landscapePosition;
        }
    }
}
