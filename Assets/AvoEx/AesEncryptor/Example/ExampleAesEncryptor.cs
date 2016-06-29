using UnityEngine;
using AvoEx;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/* See the "http://avoex.com/avoex/default-license/" for the full license governing this code. */


namespace AvoEx
{

    public class ExampleAesEncryptor : MonoBehaviour
    {
        public const float MARGIN = 10f;
        public const float WIDTH = 500f;
        public const float HEIGHT_LINE = 20f;
        public const float HEIGHT_AREA = 100f;

        public const float DEFINE_WIDTH = 120f;
        public const float VALUE_WIDTH = WIDTH - DEFINE_WIDTH;

        bool isRandomVector = true;
        byte[] customVector = AesEncryptor.GenerateIV();
        string inputValue = "I AM YOUR FATHER...";
        string encryptedValue = "";
        string decryptedValue = "";

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(MARGIN, MARGIN, WIDTH, HEIGHT_LINE));
            GUILayout.BeginHorizontal();
            bool newRandomVector = GUILayout.Toggle(isRandomVector, isRandomVector ? "Random Mode" : "customVector = ", GUILayout.Width(DEFINE_WIDTH));
            if (newRandomVector != isRandomVector)
            {
                isRandomVector = newRandomVector;
                encryptedValue = "";
                decryptedValue = "";
            }
            if (isRandomVector == false)
            {
                GUILayout.Label(System.BitConverter.ToString(customVector));
                if (GUILayout.Button("Generate Vector"))
                {
                    customVector = AesEncryptor.GenerateIV();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(MARGIN, MARGIN + HEIGHT_LINE, WIDTH, HEIGHT_AREA));
            GUILayout.BeginHorizontal();
            GUILayout.Label("inputValue = ", GUILayout.Width(DEFINE_WIDTH));
            inputValue = GUILayout.TextArea(inputValue, GUILayout.Width(VALUE_WIDTH));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(MARGIN, MARGIN + HEIGHT_AREA, WIDTH, HEIGHT_LINE));
            if (isRandomVector)
            {
                if (GUILayout.Button("encryptedValue = AesEncryptor.Encrypt(inputValue);"))
                {
                    encryptedValue = AesEncryptor.Encrypt(inputValue);
                    FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");
                    BinaryFormatter vf = new BinaryFormatter();
                    vf.Serialize(file, encryptedValue);
                    decryptedValue = "";
                }
            }
            else
            {
                if (GUILayout.Button("encryptedValue = AesEncryptor.EncryptIV(inputValue, customVector);"))
                {
                    encryptedValue = AesEncryptor.EncryptIV(inputValue, customVector);
                    decryptedValue = "";
                }
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(MARGIN, MARGIN + HEIGHT_AREA + HEIGHT_LINE, WIDTH, HEIGHT_AREA));
            GUILayout.BeginHorizontal();
            GUILayout.Label("encryptedValue = ", GUILayout.Width(DEFINE_WIDTH));
            GUILayout.Label(encryptedValue);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(MARGIN, MARGIN + HEIGHT_AREA + HEIGHT_LINE + HEIGHT_AREA, WIDTH, HEIGHT_LINE));
            if (isRandomVector)
            {
                if (GUILayout.Button("decryptedValue = AesEncryptor.DecryptString(encryptedValue);"))
                {
                    if (string.IsNullOrEmpty(encryptedValue))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
                        encryptedValue = (string)bf.Deserialize(file);
                        // = File.ReadAllText(Application.persistentDataPath + "/savedGames.json");
                    }
                    decryptedValue = AesEncryptor.DecryptString(encryptedValue);
                }
            }
            else
            {
                if (GUILayout.Button("decryptedValue = AesEncryptor.DecryptIV(encryptedValue, customVector);"))
                {
                    decryptedValue = AesEncryptor.DecryptIV(encryptedValue, customVector);
                }
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(MARGIN, MARGIN + HEIGHT_AREA + HEIGHT_LINE + HEIGHT_AREA + HEIGHT_LINE, WIDTH, HEIGHT_AREA));
            GUILayout.BeginHorizontal();
            GUILayout.Label("decryptedValue = ", GUILayout.Width(DEFINE_WIDTH));
            GUILayout.Label(decryptedValue);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }

}