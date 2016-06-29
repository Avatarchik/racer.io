using UnityEngine;
using System.Collections;

public class CarSettings : MonoBehaviour
{
    static CarSettings _instance;

    public static CarSettings Instance{ get { return _instance; } }

    public float DriftAmount;

    public float MaxTorque;

    public float MaxSpeed;
    public float PlayerAcc;
    public float PlayerTorque;

    public float EasyAIAcc;
    public float EasyAITorque;

    public float NormalAIAcc;
    public float NormalAITorque;

    public float HardAIAcc;
    public float HardAITorque;

    public float WanderAngleChange;
    public const float SlowingRadius = 10.0f;

    public Vector2 EscapeFromSeaDir = new Vector2(0.6f, 0.8f);

    public float IdleAnimTreshold;

    public float SightRange;
    public float HealthLimitPerc;
    public int HealthLostInSea;

    void Awake()
    {
        Application.targetFrameRate = 60;

        _instance = this;
    }

    void OnDestroy()
    {
        _instance = null;
    }
}
