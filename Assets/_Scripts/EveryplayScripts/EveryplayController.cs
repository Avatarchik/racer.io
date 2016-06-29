using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EveryplayController : MonoBehaviour
{
    Texture2D _thumbnailTexture;

    public Image RecON, RecOFF;

    static bool _isActive;

    static EveryplayController _instance;

    public static EveryplayController Instance { get { return _instance; } }

    // Use this for initialization
    void Awake()
    {
        _instance = this;

        Everyplay.SetLowMemoryDevice(true);
        Everyplay.SetDisableSingleCoreDevices(true);
    }

    void Start()
    {
        Everyplay.ReadyForRecording += OnReadyForRecording;
        Everyplay.RecordingStarted += OnRecordingStarted;
        Everyplay.RecordingStopped += OnRecordingFinished;
    }

    void OnDisable()
    {
        Everyplay.ReadyForRecording -= OnReadyForRecording;
        Everyplay.RecordingStarted -= OnRecordingStarted;
        Everyplay.RecordingStopped -= OnRecordingFinished;
    }

    public void TriggerEveryplay()
    {
        if (Everyplay.IsSupported())
        {
            if (RecON.gameObject.activeSelf)
            {
                SetActive(false);
            }
            else
            {
                SetActive(true);
            }
        }
    }

    public void SetActive(bool isActive)
    {
        _isActive = isActive;
        RecON.gameObject.SetActive(isActive);
        RecOFF.gameObject.SetActive(!isActive);
    }

    void OnReadyForRecording(bool recordingEnabled)
    {
    }

    public static void StartRecording()
    {
        if (_isActive && Everyplay.IsSupported() && Everyplay.IsRecordingSupported())
        {
            Everyplay.StartRecording();
        }
    }

    public static void StopRecording()
    {
        Everyplay.SetMetadata("score", MainMenuScoreController.Instance.PlayerScore);
        if (_isActive && Everyplay.IsSupported() && Everyplay.IsRecordingSupported() && Everyplay.IsRecording())
        {
            Everyplay.StopRecording();
            Everyplay.ShowSharingModal();
        }
    }

    public void ShowEveryplay()
    {
        Everyplay.ShowSharingModal();
    }

    void OnRecordingStarted()
    {
        _thumbnailTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.DXT5, false);
        Everyplay.SetThumbnailTargetTexture(_thumbnailTexture);
        StartCoroutine(TakeThumbnailInFewSeconds());
    }

    void OnRecordingFinished()
    {
        Debug.Log("recording finished");
        //TODO:Burada share buttonunu aç
        ShowEveryplay();
    }


    IEnumerator TakeThumbnailInFewSeconds()
    {
        yield return new WaitForSeconds(1);
        Everyplay.TakeThumbnail();
    }

}
