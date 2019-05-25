using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroBoardScreen : MonoBehaviour, IScreen
{
    public void Hide()
    {
        GetComponent<Canvas>().enabled = false;
    }

    public void Show()
    {
        GetComponent<Canvas>().enabled = true;
    }
}
