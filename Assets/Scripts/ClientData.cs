using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Firebase;
//using Firebase.Auth;

public class ClientData : MonoBehaviour
{
//    public bool LoggedIn { get; private set; } = false;
//    public bool IsFirebaseReady { get; private set; } = false;
//    public int Score { get; private set; }

//    FirebaseApp firebaseApp;
//    FirebaseUser user;
//    FirebaseAuth auth;
//    // Start is called before the first frame update
//    void Start()
//    {
//        //ProcessAuthentication();
        
//    }

    
//    void ProcessAuthentication()
//    {

//        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
//            var dependencyStatus = task.Result;
//            if (dependencyStatus == DependencyStatus.Available)
//            {
//                // Create and hold a reference to your FirebaseApp,
//                // where app is a Firebase.FirebaseApp property of your application class.
//                firebaseApp = FirebaseApp.DefaultInstance;
//                IsFirebaseReady = true;
//                InitializeFirebase();
//                // Set a flag here to indicate whether Firebase is ready to use by your app.
//            }
//            else
//            {
//                Debug.LogError(System.String.Format(
//                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
//                // Firebase Unity SDK is not safe to use here.
//            }
//        });
        
//    }
//void InitializeFirebase()
//{
//    auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
//    auth.StateChanged += AuthStateChanged;
//    AuthStateChanged(this, null);
//}

//void AuthStateChanged(object sender, System.EventArgs eventArgs)
//{
//    if (auth.CurrentUser != user)
//    {
//        bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
//        if (!signedIn && user != null)
//        {
//            Debug.Log("Signed out " + user.UserId);
//        }
//        user = auth.CurrentUser;
//        if (signedIn)
//        {
//            Debug.Log("Signed in " + user.UserId);
//            // displayName = user.DisplayName ?? "";
//            // emailAddress = user.Email ?? "";
//            // photoUrl = user.PhotoUrl ?? "";
//        }
//    }
//}
}
