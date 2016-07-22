using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CameraFollowScript : MonoBehaviour
{
    static CameraFollowScript _instance;

    public static CameraFollowScript Instance { get { return _instance; } }

    public BoxCollider2D ViewCollider;
    public Rigidbody2D Rigidbody;

    [HideInInspector]
    public CombatCarScript TargetCar;

    public float DampTime;

    public float VelocityCoef;

    Vector3 _velocity;

    void Awake()
    {
        _instance = this;

        InitViewColliderBounds();
    }

    void InitViewColliderBounds()
    {
        Bounds bound = CameraFrustum.GetCameraBoundsAtDepth(0);

        ViewCollider.size = new Vector2(bound.size.x, bound.size.y);
    }

    public void FixedUpdateFrame()
    {
        SelectWatchCar();
        
        if (TargetCar != null)
            FollowTarget();
    }

    void SelectWatchCar()
    {
        if (!GameManagerBase.BaseInstance.IsInWatchMode)
            TargetCar = CombatCarManagerBase.BaseInstance.GetPlayerCarScript();
        else
        {
            if (TargetCar == null)
                TargetCar = CombatCarManagerBase.BaseInstance.GetRandomActiveCarScript();
            
            if (Input.GetKeyUp(KeyCode.Mouse0))
                TargetCar = CombatCarManagerBase.BaseInstance.GetRandomActiveCarScript();

            if (TargetCar.CurHealth <= 0)
                TargetCar = CombatCarManagerBase.BaseInstance.GetRandomActiveCarScript();
        }
    }

    void FollowTarget()
    {
        Vector3 newPos = Rigidbody.position;

        newPos.x = TargetCar.MovementController.Rigidbody.position.x;
        newPos.y = TargetCar.MovementController.Rigidbody.position.y;

        newPos += (Vector3)TargetCar.MovementController.Velocity * VelocityCoef;

        Rigidbody.MovePosition(Vector3.SmoothDamp(Rigidbody.position, newPos, ref _velocity, DampTime));

    }
}