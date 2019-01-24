using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ClientData : MonoBehaviour
{
    public bool LoggedIn { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        ProcessAuthentication(false);
    }

    
    void ProcessAuthentication(bool success)
    {
        if(success)
        {
            LoggedIn = true;
        }
        else
        {
            Social.localUser.Authenticate(ProcessAuthentication);
        }
    }
}
