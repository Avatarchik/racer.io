using UnityEngine;
using System;
using System.Collections.Generic;
using Facebook.Unity;

public static class FBLogin
{

    private static readonly List<string> readPermissions = new List<string>
    {
        "public_profile ",
        "email",
        "user_friends"
    };

    private static readonly List<string> publishPermissions = new List<string>
    {
        "publish_actions"
    };

    public static void PromptForLogin(Action callback = null)
    {
        FB.LogInWithReadPermissions(readPermissions, delegate (ILoginResult result)
        {
            Debug.Log("LoginCallback");
            if (FB.IsLoggedIn)
            {
                Debug.Log("Logged in with ID: " + AccessToken.CurrentAccessToken.UserId +
                "\nGranted Permissions: " + AccessToken.CurrentAccessToken.Permissions.ToCommaSeparateList());
            } else
            {
                if (result.Error != null)
                {
                    Debug.LogError(result.Error);
                }
                Debug.Log("Not Logged In");
            }
            if (callback != null)
            {
                callback();
            }
        });
    }


    #region Util

    // Helper function to check whether the player has granted 'publish_actions'
    public static bool HavePublishActions
    {
        get
        {
            return (FB.IsLoggedIn &&
            (AccessToken.CurrentAccessToken.Permissions as List<string>).Contains("publish_actions")) ? true : false;
        }
        private set { }
    }

    #endregion
}