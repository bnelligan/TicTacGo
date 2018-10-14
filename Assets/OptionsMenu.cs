using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour, Menu {

    [SerializeField] Toggle SmallToggle;
    [SerializeField] Toggle MediumToggle;
    [SerializeField] Toggle LargeToggle;
    
    GameOptions options;
    Menu mainMenu;
    private void Awake()
    {
        mainMenu = FindObjectOfType<MainMenu>();
        options = FindObjectOfType<GameOptions>();
    }
    private void Start()
    {
        if (options.BoardSize == 3)
        {
            SmallToggle.isOn = true;
        }
        else if (options.BoardSize == 4)
        {
            MediumToggle.isOn = true;
        }
        else if (options.BoardSize == 5)
        {
            LargeToggle.isOn = true;
        }
    }

    private void Update()
    {
        if (SmallToggle.isOn)
            options.BoardSize = 3;
        else if (MediumToggle.isOn)
            options.BoardSize = 4;
        else if (LargeToggle.isOn)
            options.BoardSize = 5;
    }

    public void Show()
    {
        GetComponent<Canvas>().enabled = true;
    }
    public void Hide()
    {
        GetComponent<Canvas>().enabled = false;
    }


    public void OnClick_Menu()
    {
        Hide();
        mainMenu.Show();
    }
}
