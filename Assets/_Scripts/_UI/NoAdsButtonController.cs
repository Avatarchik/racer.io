using UnityEngine;
using System.Collections;

public class NoAdsButtonController : MonoBehaviour
{

    static NoAdsButtonController _instance;

    public static NoAdsButtonController Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    void OnDestroy()
    {
        _instance = null;
        LockButton();
    }

    public void LockButton()
    {
        gameObject.SetActive(false);
    }
}
