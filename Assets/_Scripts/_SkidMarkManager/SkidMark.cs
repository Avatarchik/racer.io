using UnityEngine;
using System.Collections;

public class SkidMark : MonoBehaviour
{
    public TrailRenderer TrailRenderer;

    float _defaultTrailTime;

    CarScript _parentCar;

    void Awake()
    {
        InitDefaultTrailTime();
    }

    void InitDefaultTrailTime()
    {
        _defaultTrailTime = TrailRenderer.time;
    }

    public void Activate(CarScript parentCar)
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
            StartCoroutine(DeactivateRoutine());
    }

    void OnCarKilled(CarScript other, DestroyReasonType reason)
    {
        OnDriftActivated(false);
    }

    IEnumerator DeactivateRoutine()
    {
        Debug.Log("deactivate routine");
        
        StopListeningEvents();

        _parentCar = null;

        transform.parent = SkidMarkManager.Instance.transform;
        
        yield return new WaitForSeconds(TrailRenderer.time);

        Deactivate();
    }

    public void Deactivate()
    {
        SkidMarkManager.Instance.SkidMarkDeactivated(this);
        
        TrailRenderer.time = 0;

        gameObject.SetActive(false);
    }
}
