using UnityEngine;
using System.Collections;

namespace AvoEx
{

    public class ScriptSample : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            string orgText = "I love you... ";
            string encText = AvoEx.AesEncryptor.Encrypt(orgText);
            Debug.Log("encText = " + encText);
            string decText = AvoEx.AesEncryptor.DecryptString(encText);
            Debug.Log("decText = " + decText);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}