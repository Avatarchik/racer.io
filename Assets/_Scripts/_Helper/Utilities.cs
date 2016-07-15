using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public enum OpenGLESV
{
    One,
    Two
}

public static class Utilities
{
    public static System.Random R;

    public static void Initialize()
    {
        R = new System.Random();
    }

    public static List<T> GetObjectTypeInArea<T>(Vector3 centerPos, float radius, List<T> objectList)
    {
        List<T> objectsInRadius = new List<T>();

        foreach (T enemy in objectList)
        {
            if (Vector2.Distance(centerPos, centerPos) <= radius)
                objectsInRadius.Add(enemy);
        }

        return objectsInRadius;
    }

    public static bool IsTargetInRange(Vector3 sourcePos, Vector3 targetPos, float radius)
    {
        if (Vector2.Distance(sourcePos, targetPos) < radius)
            return true;

        return false;
    }

    public static bool IsObjectPressed(Collider targetCollider)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (targetCollider.Raycast(ray, out hit, 100))
        {
            return true;
        }

        return false;
    }

    public static T IdentifyObjectEnum<T>(string objectname, T enumExample)
    {
        T type = (T)Enum.Parse(typeof(T), objectname);
        return type;
    }

    #region UI Region

    public static OpenGLESV GetOpenGlesVersion()
    {
        if (IsTouchPlatform())
        {
#if UNITY_IPHONE
            return OpenGLESV.Two;
            switch (UnityEngine.iOS.Device.generation)
            {
                case UnityEngine.iOS.DeviceGeneration.iPhone3GS:
                case UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen:
                case UnityEngine.iOS.DeviceGeneration.iPhone4:
                case UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen:
                case UnityEngine.iOS.DeviceGeneration.iPad1Gen:
                    return OpenGLESV.One;
                default:
                    return OpenGLESV.Two;
            }
#endif
        }
        return OpenGLESV.Two;
    }

    public static bool IsTouchPlatform()
    {
        return Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android;
    }

    #endregion

    #region Math Region

    public static float NextFloat(float minValue = 0, float maxValue = 1)
    {
        double range = maxValue - (double)minValue;
        double sample = R.NextDouble();
        double scaled = (sample * range) + minValue;

        return (float)scaled;
    }

    public static int NextInt(int minValue, int maxValue)
    {
        return R.Next(minValue, maxValue);
    }

    public static int Combination(int n, int r)
    {
        int Comb = 0;
        Comb = (int)(Factorial(n) / (Factorial(r) * Factorial(n - r)));
        return Comb;
    }

    public static long Factorial(int num)
    {
        long fact = 1;
        for (int i = 2; i <= num; i++)
        {
            fact = fact * i;
        }
        return fact;
    }

    #endregion

    #region Easing Functions

    public static IEnumerator EaseFuncPos(Rigidbody rigidBody, Vector3 targetPos, float start, float end, float curTime, float duration, Func<float, float, float, float, float> easeFunc)
    {
        Vector3 startPos = rigidBody.position;

        while (curTime < duration)
        {
            curTime += Time.deltaTime / Time.timeScale;
            float step = easeFunc(start, end, curTime, duration);
            rigidBody.position = Vector3.Lerp(startPos, targetPos, step);
            yield return null;
        }
    }

    public static IEnumerator EaseFuncPos(GameObject gameObject, Vector3 targetPos, float start, float end, float curTime, float duration, Func<float, float, float, float, float> easeFunc)
    {
        Vector3 startPos = gameObject.transform.position;

        while (curTime < duration)
        {
            curTime += Time.deltaTime / Time.timeScale;
            float step = easeFunc(start, end, curTime, duration);
            gameObject.transform.position = Vector3.Lerp(startPos, targetPos, step);
            yield return null;
        }
    }

    public static IEnumerator EaseFuncScale(GameObject gameObject, Vector3 targetScale, float start, float end, float curTime, float duration, Func<float, float, float, float, float> easeFunc)
    {
        Vector3 startScale = gameObject.transform.localScale;

        while (curTime < duration)
        {
            curTime += Time.deltaTime / Time.timeScale;
            float step = easeFunc(start, end, curTime, duration);
            gameObject.transform.localScale = Vector3.Lerp(startScale, targetScale, step);
            yield return null;
        }
    }

    public static IEnumerator EaseFuncRotate(GameObject gameObject, float amount, bool isClockWise, float start, float end, float curTime, float duration, Func<float, float, float, float, float> easeFunc)
    {
        Vector3 startEulerAngles = gameObject.transform.localEulerAngles;
        Vector3 targetEulerAngles = startEulerAngles;

        if (isClockWise)
            targetEulerAngles.z += amount;
        else
            targetEulerAngles.z -= amount;

        while (curTime < duration)
        {
            curTime += Time.deltaTime / Time.timeScale;
            float step = easeFunc(start, end, curTime, duration);
            gameObject.transform.localEulerAngles = Vector3.Lerp(startEulerAngles, targetEulerAngles, step);
            yield return null;
        }
    }

    public static IEnumerator EaseFuncRotate(Rigidbody rigidBody, float amount, bool isClockWise, float start, float end, float curTime, float duration, Func<float, float, float, float, float> easeFunc)
    {
        Vector3 startEulerAngles = rigidBody.rotation.eulerAngles;
        Vector3 targetEulerAngles = startEulerAngles;

        if (isClockWise)
            targetEulerAngles.z += amount;
        else
            targetEulerAngles.z -= amount;

        while (curTime < duration)
        {
            curTime += Time.deltaTime / Time.timeScale;
            float step = easeFunc(start, end, curTime, duration);
            rigidBody.rotation = Quaternion.Euler(Vector3.Lerp(startEulerAngles, targetEulerAngles, step));
            yield return null;
        }
    }

    public static float Linear(float start, float end, float curTime, float duration)
    {
        curTime /= duration;
        end -= start;
        return end * curTime + start;
    }

    public static float EaseInCubic(float start, float end, float curTime, float duration)
    {
        curTime /= duration;
        end -= start;
        return end * curTime * curTime * curTime + start;
    }

    public static float EaseInOutCubic(float start, float end, float curTime, float duration)
    {
        curTime = curTime / duration - 1;
        end -= start;
        return end * (curTime * curTime * curTime + 1) + start;
    }

    public static Vector3 EaseInOutCubic(Vector3 start, Vector3 end, float curTime, float duration)
    {
        curTime = curTime / duration - 1;
        end -= start;
        return end * (curTime * curTime * curTime + 1) + start;
    }

    public static float EaseInSine(float start, float end, float curTime, float duration)
    {
        end -= start;
        return -end * Mathf.Cos(curTime / duration / 1 * (Mathf.PI / 2)) + end + start;
    }

    public static float EaseOutSine(float start, float end, float curTime, float duration)
    {
        end -= start;
        return end * Mathf.Sin(curTime / duration / 1 * (Mathf.PI / 2)) + start;
    }

    public static float EaseInOutSine(float start, float end, float curTime, float duration)
    {
        end -= start;
        return -end / 2 * (Mathf.Cos(Mathf.PI * curTime / duration / 1) - 1) + start;
    }

    public static float EaseInExpo(float start, float end, float curTime, float duration)
    {
        end -= start;
        return end * Mathf.Pow(2, 10 * (curTime / duration / 1 - 1)) + start;
    }

    public static float EaseOutExpo(float start, float end, float curTime, float duration)
    {
        end -= start;
        return end * (-Mathf.Pow(2, -10 * curTime / duration / 1) + 1) + start;
    }

    public static float EaseInOutExpo(float start, float end, float curTime, float duration)
    {
        curTime /= .5f;
        end -= start;
        if (curTime < 1)
            return end / 2 * Mathf.Pow(2, 10 * (curTime / duration - 1)) + start;
        curTime--;
        return end / 2 * (-Mathf.Pow(2, -10 * curTime / duration) + 2) + start;
    }

    public static float EaseInOutBack(float start, float end, float curTime, float duration)
    {
        curTime /= duration;
        //Debug.Log(end + " " + start + " " +curTime);
        float s = 1.70158f;
        end -= start;
        //curTime /= duration;
        curTime /= .5f;
        if ((curTime) < 1)
        {
            s *= (1.525f);
            return end / 2 * (curTime * curTime * (((s) + 1) * curTime - s)) + start;
        }
        curTime -= 2;
        s *= (1.525f);
        return end / 2 * ((curTime) * curTime * (((s) + 1) * curTime + s) + 2) + start;
    }

    public static float EaseOutCubic(float start, float end, float curTime, float duration)
    {
        //curTime--;
        curTime = curTime / duration - 1;
        end -= start;
        //(t=t/d-1)

        return end * (curTime * curTime * curTime + 1) + start;
    }

    public static Vector3 EaseOutCubic(Vector3 start, Vector3 end, float curTime, float duration)
    {
        //curTime--;
        curTime = curTime / duration - 1;
        end -= start;
        //(t=t/d-1)

        return end * (curTime * curTime * curTime + 1) + start;
    }

    #endregion

    #region Collision Region

    public static RaycastHit RayCast(Vector3 origin, Vector3 direction, float distance, LayerMask layerMask, bool drawRay = true)
    {
        RaycastHit rayCastHit;

        Physics.Raycast(origin, direction, out rayCastHit, distance, layerMask);

        if (drawRay)
        {
            if (rayCastHit.collider == null)
                Debug.DrawRay(origin, direction * distance, Color.black, 100);
            else
                Debug.DrawRay(origin, direction * distance, Color.green, 100);
        }

        return rayCastHit;
    }

    #endregion

    #region Audio Region

    public static IEnumerator WaitForSoundFinish(AudioSource source, Action callback)
    {
        yield return new WaitForSeconds(0.05f);

        while (source.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }
        if (callback != null)
            callback();
    }

    #endregion

    public static string ConvertDateToString(DateTime dt)
    {
        return dt.ToString("yyyy-MM-dd");
    }

    public static DateTime ConvertStringToDate(string date)
    {
        return DateTime.ParseExact(date, "yyyy-MM-dd", null);
    }

    public static int[] RandomIntinAmount(int amount, int minNumber, int maxNumber)
    {
        R = new System.Random();
        var randomArr = new int[amount];
        for (int i = 0; i < amount; i++)
        {
            Debug.Log(randomArr[i]);

            randomArr[i] = R.Next(minNumber, maxNumber);
        }
        return randomArr;
    }

    public static T GetRandomEnum<T>(int startIndex = 0, int endIndex = 0)
    {
        Array A = Enum.GetValues(typeof(T));
        var V = (T)A.GetValue(UnityEngine.Random.Range(startIndex, A.Length - endIndex));
        return V;
    }
}
