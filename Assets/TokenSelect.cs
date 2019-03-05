using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokenSelect : MonoBehaviour
{
    [SerializeField]
    Button btnSwapLeft;
    [SerializeField]
    Button btnSwapRight;
    [SerializeField]
    Image imgDisplayToken;

    public Sprite SelectedToken { get { return AvailableTokens[idxSelected]; } }
    private Sprite[] AvailableTokens;
    int idxSelected = 0;
    
    private void Awake()
    {
        AvailableTokens = Resources.LoadAll<Sprite>("Sprites/Tokens/Active/");
        btnSwapLeft.onClick.AddListener(OnClick_SwapLeft);
        btnSwapRight.onClick.AddListener(OnClick_SwapRight);
    }
    private void Start()
    {
        SetToken();
    }

    public void OnClick_SwapRight()
    {
        idxSelected++;
        if (idxSelected >= AvailableTokens.Length)
        {
            idxSelected = 0;
        }
        SetToken();
    }

    public void OnClick_SwapLeft()
    {
        idxSelected--;
        if (idxSelected < 0)
        {
            idxSelected = AvailableTokens.Length - 1;
        }
        SetToken();
    }

    private void SetToken()
    {
        if(AvailableTokens.Length > 0)
        {
            Debug.Log("name" + SelectedToken.name);
            imgDisplayToken.sprite = SelectedToken;
        }
        else
        {
            Debug.LogError("No tokens available!");
        }
    }
}
