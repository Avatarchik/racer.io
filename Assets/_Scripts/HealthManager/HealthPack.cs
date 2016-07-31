using UnityEngine;
using System.Collections;

public class HealthPack : MonoBehaviour
{
    public int HealthAmount;
    public float DampTime;

    [HideInInspector]
    public bool IsTakenByPlayer;

    bool _isCollected;
    CarBase _collectedCar;

    public void Deactivate()
    {
        transform.localPosition = Vector2.zero;

        gameObject.SetActive(false);
    }

    public void Activate(Vector2 spawnPos)
    {
        _isCollected = false;
        _collectedCar = null;
        
        IsTakenByPlayer = false;

        transform.position = spawnPos;

        gameObject.SetActive(true);
    }      

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == (int)LayerEnum.CarMagnetZone)
        {
            EnteredMagnetZone(other);
        }
    }

    void EnteredMagnetZone(Collider2D other)
    {
        if (_isCollected)
            return;
        
        _collectedCar = other.transform.parent.GetComponent<CarBase>();

        _isCollected = true;

        StartCoroutine(MoveToTarget());
    }

    IEnumerator MoveToTarget()
    {
        Vector2 velocity = Vector2.zero;
        
        while(Vector2.Distance(transform.position, _collectedCar.transform.position) > 2f)
        {
            transform.position = Vector2.SmoothDamp(transform.position, _collectedCar.transform.position, ref velocity, DampTime);
            
            yield return new WaitForFixedUpdate();
        }

        _collectedCar.CollectedHealthPack(HealthAmount);

        
        IsTakenByPlayer = true;
    }
}
