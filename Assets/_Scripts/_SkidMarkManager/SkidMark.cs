using UnityEngine;
using System.Collections;

public class SkidMark : MonoBehaviour
{
    public TrailRenderer TrailRenderer;

    float _defaultTrailTime;

    CarBase _parentCar;

    void Awake()
    {
        InitDefaultTrailTime();
    }

    void InitDefaultTrailTime()
    {
        _defaultTrailTime = TrailRenderer.time;
    }

    public void Activate(CarBase parentCar)
    {
        _parentCar = parentCar;
        
        transform.parent = parentCar.TrailCarrier;
        transform.localPosition = Vector3.zero;
        
        TrailRenderer.time = _defaultTrailTime;

        gameObject.SetActive(true);

        StartListeningEvents();
    }

    void StartListeningEvents()
    {
        _parentCar.MovementController.OnDriftActive += OnDriftActivated;
        _parentCar.OnGetKilled += OnCarKilled;
    }

    void StopListeningEvents()
    {
        _parentCar.MovementController.OnDriftActive -= OnDriftActivated;
        _parentCar.OnGetKilled -= OnCarKilled;
    }

    void OnDriftActivated(bool isActive)
    {
        if (!isActive)
        {
            if (gameObject.activeSelf)
                StartCoroutine(DeactivateRoutine());
            else
            {
                _parentCar = null;

                transform.parent = SkidMarkManager.Instance.transform;

                Deactivate();
            }
        }
    }

    void OnCarKilled(CarBase other, DestroyReasonType reason)
    {
        OnDriftActivated(false);
    }

    IEnumerator DeactivateRoutine()
    {
        StopListeningEvents();

        _parentCar = null;

        transform.parent = SkidMarkManager.Instance.transform;
        
        yield return new WaitForSeconds(TrailRenderer.time);

        TrailRenderer.time = 0;

        yield return new WaitForSeconds(0.1f);

        Deactivate();
    }

    public void Deactivate()
    {
        SkidMarkManager.Instance.SkidMarkDeactivated(this);

        gameObject.SetActive(false);
    }
}
