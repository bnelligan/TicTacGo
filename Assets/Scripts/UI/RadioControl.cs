using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadioControl : MonoBehaviour {

    [SerializeField] List<RadioToggle> radios;
    RadioToggle selRadio;

	// Use this for initialization
	void Start () {
		if(radios == null)
        {
            radios = new List<RadioToggle>(GetComponentsInChildren<RadioToggle>());
            selRadio = radios[0];
            selRadio.isOn = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
        ForceSingleToggle();
	}

    void OnRadioClick(RadioToggle sender)
    {
        selRadio = sender;
    }

    void ForceSingleToggle()
    {
        foreach(RadioToggle radio in radios)
        {
            if (radio == selRadio)
            {
                radio.isOn = true;
            }
            else
            {
                radio.isOn = false;
            }
        }
    }
}
