using UnityEngine;
using System.Collections;

public class AmmoBase : MonoBehaviour
{
    public Renderer Renderer;
    public Rigidbody2D Rigidbody;
    
    protected Weapon_AmmoBased _parentWeapon;

    public Weapon_AmmoBased ParentWeapon{ get { return _parentWeapon; } }

    public AudioSource AmmoSound;
    public TrailRenderer TrailRenderer;

    protected IEnumerator _moveRoutine;

    protected bool _hitCar;

    public void InitAmmo(Weapon_AmmoBased parentWeapon)
    {
        _parentWeapon = parentWeapon;

        Deactivate();
    }

    public virtual void Activate(Transform ammoSlot)
    {
        transform.parent = null;
        
        transform.position = ammoSlot.position;
        transform.localEulerAngles = ammoSlot.localEulerAngles;

        SetRendererActive(true);

        gameObject.SetActive(true);

        StartMoving();
    }

    void StartMoving()
    {
        if (_moveRoutine != null)
            StopCoroutine(_moveRoutine);

        _moveRoutine = MoveProgress();
        StartCoroutine(_moveRoutine);
    }

    protected virtual IEnumerator MoveProgress()
    {
        AmmoSound.Play();

        float trailTime = TrailRenderer.time;
        TrailRenderer.time = 0f;

        yield return new WaitForFixedUpdate();

        TrailRenderer.time = trailTime;

        float distanceTaken = 0;

        while (distanceTaken < _parentWeapon.WeaponRange && !_hitCar)
        {
            Vector3 deltaDistance = _parentWeapon.AmmoSpeed * transform.right * Time.fixedDeltaTime;
            
            Vector3 newPos = Rigidbody.position;
            newPos += deltaDistance;
            
            Rigidbody.MovePosition(newPos);

            distanceTaken += deltaDistance.magnitude;
            
            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(DeactivateProgress());
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == (int)LayerEnum.Car)
        {
            CarBase targetCar = other.GetComponent<CarBase>();

            if (targetCar == _parentWeapon.WeaponController.MyCar || targetCar.IsInGhostMode)
                return;

            targetCar.GetHit(_parentWeapon);

            _hitCar = true;
        }
    }

    protected virtual IEnumerator DeactivateProgress()
    {
        Deactivate();

        yield break;
    }


    public virtual void Deactivate()
    {
        transform.parent = _parentWeapon.AmmoCarrier.transform;

        _parentWeapon.AddToDeactiveList(this);

        gameObject.SetActive(false);
    }

    protected void SetRendererActive(bool isActive)
    {
        Renderer.enabled = isActive;
    }
}
