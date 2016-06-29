using UnityEngine;
using System.Collections;

public class InternetChecker : MonoBehaviour
{
    const float CheckTimer = 5f;

    public static bool IsconnectedToInternet = false;

    void Awake()
    {
        StartCoroutine(PingInternet());
    }

    IEnumerator PingInternet()
    {
        while (true)
        {
            yield return StartCoroutine(PingGOOGLE());
            if (IsconnectedToInternet)
                yield return new WaitForSeconds(200f);
            else
                yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator PingGOOGLE()
    {
        WWW www = new WWW("http://www.google.com");

        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            IsconnectedToInternet = true;
        } else
        {
            IsconnectedToInternet = false;
        }
    }

    public static IEnumerator GetAdInterval(System.Action<int> callback)
    {
        WWW www = new WWW("http://www.mildmania.info/planesadinterval.txt");

        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {

            int curAdInterval = -1;
            if (int.TryParse(www.text, out curAdInterval))
            {
                callback(curAdInterval);
            }
        } else
        {
            callback(GetIntervalFromResources());
        }
    }

    public static int GetIntervalFromResources()
    {
        if (HeyzapAdManager.Instance.AD_INTERVAL_IN_SECONDS < 0)
        {
            TextAsset ta = Resources.Load("AdInterval") as TextAsset;
            return int.Parse(ta.text);
        }
        return -1;
    }

}