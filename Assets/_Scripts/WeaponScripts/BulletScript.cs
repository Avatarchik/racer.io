using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour
{
    protected WeaponController _weaponController;

    public WeaponController WeaponController { get { return _weaponController; } }

    public AudioSource AudioSource;
    public TrailRenderer TrailRenderer;

    public Rigidbody2D Rigidbody;

    Transform _oldParent = null;
    IEnumerator _moveRoutine;

    [HideInInspector]
    public Vector2 ShotPos;
    [HideInInspector]
    public int Damage;
    [HideInInspector]
    public bool IsCarHit;

    public void Deactivate()
    {
        if (_moveRoutine != null)
        {
            StopCoroutine(_moveRoutine);
            _moveRoutine = null;
        }

        gameObject.SetActive(false);

        if (_oldParent != null)
            transform.parent = _oldParent;

        transform.localPosition = Vector2.zero;
        transform.localEulerAngles = new Vector3(0f, 0f, 0f);
    }

    public void Activate(WeaponController weaponController, Transform ammoSlot, int BulletSpeed, int damage)
    {
        IsCarHit = false;

        _weaponController = weaponController;
        Damage = damage;

        _oldParent = transform.parent;

        transform.parent = ammoSlot;
        transform.localPosition = Vector2.zero;
        transform.localEulerAngles = Vector3.zero;

        transform.parent = null;
        ShotPos = transform.position;

        gameObject.SetActive(true);

        if (_moveRoutine != null)
            return;

        _moveRoutine = MoveRoutine(BulletSpeed);
        StartCoroutine(_moveRoutine);

        AudioSource.Play();
    }

    protected virtual IEnumerator MoveRoutine(int BulletSpeed)
    {
        float initTime = TrailRenderer.time;
        TrailRenderer.time = 0f;

        yield return new WaitForFixedUpdate();

        TrailRenderer.time = initTime;

        while (true)
        {
            float delta = BulletSpeed * Time.fixedDeltaTime;

            Rigidbody.position += (Vector2)transform.right * delta;

            yield return new WaitForFixedUpdate();

            if (Vector2.Distance(transform.position, ShotPos) >= _weaponController.WeaponInfo.FireDistance || IsCarHit)
            {
                _weaponController.AddBulletToDeactiveList(this);

                Deactivate();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == (int)LayerEnum.Car)
        {
            CarScript targetCar = other.GetComponent<CarScript>();

            if (targetCar == _weaponController.Car || targetCar.IsInGhostMode)
                return;

            targetCar.GetHit(this);

            IsCarHit = true;
        }
    }
}
