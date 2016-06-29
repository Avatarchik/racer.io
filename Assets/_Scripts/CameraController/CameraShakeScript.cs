using UnityEngine;
using System.Collections;

public class CameraShakeScript : MonoBehaviour 
{
    static CameraShakeScript _instance;
    public static CameraShakeScript Instance { get { return _instance; } }

    public float DefautShakeAmount;
    public float DefaultShakeDuration;

    [HideInInspector]
    public float EaseOutDuration;

    float _curShakeAmount;
    float _curShakeDuration;

    Transform _cameraCarrier;

    IEnumerator _shakeRoutine;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        InitCameraCarrier();
    }

    void InitCameraCarrier()
    {
        _cameraCarrier = transform.parent;
    }

    public void ShakeCamera(float duration = 0, float amount = 0)
    {
        _curShakeAmount = amount;
        _curShakeDuration = duration;

        if (_curShakeAmount == 0)
            _curShakeAmount = DefautShakeAmount;

        if (_curShakeDuration == 0)
            _curShakeDuration = DefaultShakeDuration;

        _shakeRoutine = ShakeRoutine();

        StartCoroutine(_shakeRoutine);
    }

    IEnumerator ShakeRoutine()
    {
        float shake = _curShakeDuration;
        float shakeAmount = _curShakeAmount;

        while (shake > 0)
        {
            if (shake <= EaseOutDuration)
                shakeAmount = Utilities.EaseOutCubic(_curShakeAmount, 0f, EaseOutDuration - shake, EaseOutDuration);

            Vector2 newLocal =  Random.insideUnitSphere * shakeAmount;

            _cameraCarrier.localPosition = (Vector3)newLocal;

            shake -= Time.fixedDeltaTime;

            yield return null;
        }

        _cameraCarrier.localPosition = Vector3.zero;
    }
}
