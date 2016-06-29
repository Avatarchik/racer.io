using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CarSelectScrollPanel : MonoBehaviour
{
    public ScrollRect ScrollRect;
    public GameObject FirstPlane, LastPlane;
    public Transform LeftBorder, RightBorder;

    public float _lastNormalizedPosition;

    void Awake()
    {
        ScrollRect.horizontalNormalizedPosition = 0f;
        _lastNormalizedPosition = ScrollRect.horizontalNormalizedPosition;
    }

    public void OnValueChanged()
    {
        if (FirstPlane.transform.position.x > LeftBorder.transform.position.x || LastPlane.transform.position.x < RightBorder.transform.position.x)
            ScrollRect.horizontalNormalizedPosition = _lastNormalizedPosition;
        else
            _lastNormalizedPosition = ScrollRect.horizontalNormalizedPosition;
    }
}
