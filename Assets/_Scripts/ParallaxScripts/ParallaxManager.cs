using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ParallaxManager : MonoBehaviour 
{
    public List<ParallaxLayer> ParallaxLayerList;

    public List<Transform> ParallaxObjectList;

    Vector2 _deltaMovement;
    Vector2 _prevCameraPos;

    void Start()
    {
        _prevCameraPos = (Vector2)CameraFollowScript.Instance.transform.position;

        InitLayers();
    }

    void InitLayers()
    {
        foreach(Transform tr in ParallaxObjectList)
        {
            int index = Utilities.NextInt(0, ParallaxLayerList.Count);

            tr.transform.parent = ParallaxLayerList[index].transform;
        }
    }

    void Update()
    {
        UpdateDeltaMovement();
        UpdateLayers();

    }

    void UpdateDeltaMovement()
    {
        Vector2 cameraCurPos = (Vector2)CameraFollowScript.Instance.transform.position;
        
        _deltaMovement = cameraCurPos - _prevCameraPos;

        _prevCameraPos = cameraCurPos;
    }

    void UpdateLayers()
    {
        foreach (ParallaxLayer layer in ParallaxLayerList)
            layer.UpdateLayer(_deltaMovement);
    }
}
