using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class RadioToggle : MonoBehaviour
{

    Toggle toggle;
    List<RadioToggle> siblings;

    public bool isOn
    {
        get { return toggle.isOn; }
        set { toggle.isOn = value; }
    }

    // Use this for initialization
    void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnClick);
        siblings = new List<RadioToggle>();

        Transform parent = transform.parent;
        for(int s = 0; s < parent.childCount; s++)
        {
            if(s != transform.GetSiblingIndex())
            {
                RadioToggle radSibling = parent.GetChild(s).GetComponent<RadioToggle>();
                if(radSibling)
                {
                    siblings.Add(radSibling);
                }
            }
        }
    }

    public void OnClick(bool toggleValue)
    {
        if (toggleValue)
        {
            foreach (RadioToggle r in siblings)
            {
                r.isOn = false;
            }
        }
        else
        {
            if(!IsSiblingOn())
            {
                isOn = true;
            }
        }
    }
    private bool IsSiblingOn()
    {
        bool foundSelected = false;
        foreach(RadioToggle r in siblings)
        {
            if (r.isOn)
            {
                foundSelected = true;
            }
        }
        return foundSelected;
    }
}
