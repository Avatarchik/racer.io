using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class MMTweenPosition : MMUITweener
{
    public Vector3 From, To;
    public bool UseWorldPosition, UseRigidbody;

    RectTransform myTransform;
    Rigidbody myRigidbody;
    Rigidbody2D myRigidbody2D;

    protected override void Wake()
    {
        if (UseWorldPosition && UseRigidbody)
            CheckForRigidbodyAndCollider();
        else
            myTransform = gameObject.GetComponent<RectTransform>();

        if (UseWorldPosition)
        {
            if(UseRigidbody)
            {
                if (myRigidbody != null)
                    myRigidbody.position = From;
                else
                    myRigidbody2D.position = From;
            }
            else
                myTransform.position = From;
        }
        else
            myTransform.localPosition = From;
    }

    void CheckForRigidbodyAndCollider()
    {
        //TODO: Şimdilik sadece 3D rigidbody check edilip ekleniyor, ilerde projenin 2D veya 3D olmasına bağlı olarak check işlemi yapılmalı
        if (gameObject.GetComponent<Rigidbody>() == null)
        {
            myRigidbody = gameObject.AddComponent<Rigidbody>();
            myRigidbody.isKinematic = true;
        }
        else
            myRigidbody = gameObject.GetComponent<Rigidbody>();

        if (gameObject.GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();
    }

    protected override void PlayAnim(float clampedValue)
    {
        Vector3 diff = To - From;
        Vector3 delta = diff * clampedValue;

        if (UseWorldPosition)
        {
            if (UseRigidbody)
            {
                if (myRigidbody != null)
                    myRigidbody.position = From + delta;
                else
                    myRigidbody2D.position = From + delta;
            }
            else
                myTransform.position = From + delta;
        }
        else
            myTransform.localPosition = From + delta;
    }

    protected override void Finish()
    {
    }

    protected override void Kill()
    {
    }

    #region ContextMenu
    [ContextMenu("Set FROM")]
    void SetFrom()
    {
        if (UseWorldPosition)
            From = GetComponent<RectTransform>().position;
        else
            From = GetComponent<RectTransform>().localPosition;
    }

    [ContextMenu("Set TO")]
    void SetTo()
    {
        if (UseWorldPosition)
            To = GetComponent<RectTransform>().position;
        else
            To = GetComponent<RectTransform>().localPosition;
    }

    [ContextMenu("Assume FROM")]
    void AssumeFrom()
    {
        if (UseWorldPosition)
            GetComponent<RectTransform>().position= From;
        else
            GetComponent<RectTransform>().localPosition = From;
    }

    [ContextMenu("Assume TO")]
    void AssumeTo()
    {
        if (UseWorldPosition)
            GetComponent<RectTransform>().position = To;
        else
            GetComponent<RectTransform>().localPosition = To;
    } 
    #endregion
}
