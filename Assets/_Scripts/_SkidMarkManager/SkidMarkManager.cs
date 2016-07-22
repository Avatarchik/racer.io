using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkidMarkManager : MonoBehaviour
{
    static SkidMarkManager _instance;

    public static SkidMarkManager Instance{ get { return _instance; } }

    public GameObject SkidMarkPrefab;
    public int MaxSkidMarkCount;

    List<SkidMark> _deactiveSkidMarkList;
    List<SkidMark> _activeSkidMarkList;

    void Awake()
    {
        _instance = this;

        GenerateSkidMarks();
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void GenerateSkidMarks()
    {
        _deactiveSkidMarkList = new List<SkidMark>();
        _activeSkidMarkList = new List<SkidMark>();

        for (int i = 0; i < MaxSkidMarkCount; i++)
        {
            GameObject skidMarkObject = GameObject.Instantiate(SkidMarkPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            SkidMark skidMark = skidMarkObject.GetComponent<SkidMark>();

            skidMark.transform.parent = transform;

            skidMark.Deactivate();
        }
    }

    public void StartedDrifting(CarBase car)
    {
        if (_deactiveSkidMarkList.Count == 0)
            return;

        SkidMark skidMark = _deactiveSkidMarkList[0];

        _deactiveSkidMarkList.Remove(skidMark);
        _activeSkidMarkList.Add(skidMark);

        skidMark.Activate(car);

    }

    public void SkidMarkDeactivated(SkidMark skidMark)
    {
        _activeSkidMarkList.Remove(skidMark);
        _deactiveSkidMarkList.Add(skidMark);
    }

}
