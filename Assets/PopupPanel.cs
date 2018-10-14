using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupPanel : MonoBehaviour {
    public bool StartActive = false;
    #region Panel Elements
    public GameObject BtnRematch;
    public GameObject BtnMenu;
    public GameObject TxtTitle;
    #endregion

    #region Element Accessors
    public string TitleText
    {
        get { return TxtTitle.GetComponent<Text>().text; }
        set { TxtTitle.GetComponent<Text>().text = value; }
    }
    public string RematchText
    {
        get { return BtnRematch.GetComponentInChildren<Text>().text; }
        set { BtnRematch.GetComponentInChildren<Text>().text = value; }
    }
    #endregion
    
    private void Start()
    {
        if (StartActive)
            Show();
        else
            Hide();
    }
    public void ShowGameOver()
    {
        Show();
        TitleText = "Game Over";
    }
    public void ShowRematch()
    {
        Show();
        TitleText = "Rematch Requested";
        RematchText = "Cancel";
    }
    public void ShowCancelRematch()
    {
        Show();
        TitleText = "Rematch Canceled";
        RematchText = "Rematch";
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
