using UnityEngine;
using System.Collections;

public static class CameraFrustum
{

    public static Bounds GetCameraBoundsAtDepth(float depth)
    {
        depth -= Camera.main.transform.position.z;

        float screenHeight = GetScreenHeightAtDepth(depth);

        float screenWidth = GetScreenWidthAtDepth(depth);

        Bounds cameraBounds = new Bounds(Camera.main.transform.position, new Vector3(screenWidth, screenHeight, 0));

        return cameraBounds;
    }

    public static float GetScreenHeightAtDepth(float depth)
    {
        depth -= Camera.main.transform.position.z;

        return 2.0f * Mathf.Tan(0.5f * Camera.main.fieldOfView * Mathf.Deg2Rad) * depth;
    }

    public static float GetScreenWidthAtDepth(float depth)
    {
        depth -= Camera.main.transform.position.z;

        return 2.0f * Mathf.Tan(0.5f * Camera.main.fieldOfView * Mathf.Deg2Rad) * depth * Camera.main.aspect;
    }

    public static bool IsInCameraFrustum(Vector3 pos)
    {
        Bounds bound = CameraFollowScript.Instance.ViewCollider.bounds;
        bound.center = CameraFollowScript.Instance.transform.position;
        bound.center = new Vector3(bound.center.x, bound.center.y, pos.z);

        bound.size = new Vector3(bound.size.x, bound.size.y, 1.0f);

        return bound.Contains(pos);
    }
}