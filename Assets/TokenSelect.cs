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
    [SerializeField]
    Player targetPlayer;
    
    private void Awake()
    {
        AvailableTokens = Resources.LoadAll<Sprite>("Sprites/Tokens/Active/");
        btnSwapLeft.onClick.AddListener(OnClick_SwapLeft);
        btnSwapRight.onClick.AddListener(OnClick_SwapRight);
    }
    private void Start()
    {
        string SavedToken = PlayerPrefs.GetString($"{targetPlayer.ToString()}_Token", "NONE");
        for(int i = 0; i < AvailableTokens.Length; i++)
        {
            if(AvailableTokens[i].name == SavedToken)
            {
                idxSelected = i;
            }
        }
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
        PlayerPrefs.SetString($"{targetPlayer.ToString()}_Token", SelectedToken.name);
        if (AvailableTokens.Length > 0)
        {
            imgDisplayToken.sprite = SelectedToken;
        }
        else
        {
            Debug.LogError("No tokens available!");
        }
        PlayerPrefs.Save();
    }
}
