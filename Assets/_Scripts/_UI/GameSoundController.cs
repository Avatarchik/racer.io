using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameSoundController : MonoBehaviour
{
    public Image SoundOn, SoundOff;

    public void Execute()
    {
        if(SoundOn.gameObject.activeInHierarchy)
        {
            SoundOn.gameObject.SetActive(false);
            SoundOff.gameObject.SetActive(true);

            ChangeSound(true);
        }
        else
        {
            SoundOff.gameObject.SetActive(false);
            SoundOn.gameObject.SetActive(true);

            ChangeSound(false);
        }
    }

    void ChangeSound(bool state)
    {
        AudioListener.pause = state;
    }
}
