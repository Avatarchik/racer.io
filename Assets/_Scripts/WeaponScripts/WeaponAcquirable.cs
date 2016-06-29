using UnityEngine;
using System.Collections;

public class WeaponAcquirable : MonoBehaviour
{
    public WeaponTypeEnum WeaponType;
    public float MoveSpeed;
    public float DampTime;
    public Rigidbody2D Rigidbody;

    [HideInInspector]
    public bool IsAcquired;
    [HideInInspector]
    public int AmmoCount;
    [HideInInspector]
    public tk2dSprite Sprite;

    IEnumerator _activateRoutine;

    bool _isCollected;
    CarScript _collectedCar;

    public void Init(Transform parent)
    {
        transform.parent = parent;

        IsAcquired = false;

        _activateRoutine = null;

        Sprite = GetComponent<tk2dSprite>();

        Deactivate();
    }

    public void Activate(bool isInstantSpawn, Vector2 spawnPos, int ammoCount)
    {
        gameObject.SetActive(true);

        _isCollected = false;
        _collectedCar = null;
        IsAcquired = false;

        AmmoCount = ammoCount;

        Rigidbody.position = spawnPos;

        if (isInstantSpawn)
        {
            Sprite.color = new Color(1f, 1f, 1f, 1f);
            return;
        }

        if (_activateRoutine != null)
            StopCoroutine(_activateRoutine);

        _activateRoutine = ActivateRoutine();
        StartCoroutine(_activateRoutine);
    }

    IEnumerator ActivateRoutine()
    {
        float _time = 0f;
        Color startingColor = Sprite.color;

        while (_time < WeaponDropManager.Instance.DropTransitionDuration)
        {
            Color newColor = Color.Lerp(startingColor, new Color(1f, 1f, 1f, 1f), _time / WeaponDropManager.Instance.DropTransitionDuration);

            Sprite.color = newColor;

            yield return new WaitForFixedUpdate();
            _time += Time.fixedDeltaTime;
        }

        Sprite.color = new Color(1f, 1f, 1f, 1f);
    }

    public void Deactivate()
    {
        if (_activateRoutine != null)
            StopCoroutine(_activateRoutine);

        Rigidbody.position = Vector2.zero;
        gameObject.SetActive(false);
        Sprite.color = new Color(1f, 1f, 1f, 0f);
    }

    public void Move()
    {
        if (gameObject.activeInHierarchy)
            Rigidbody.position -= new Vector2(0f, MoveSpeed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == (int)LayerEnum.CarMagnetZone)
        {
            EnteredMagnetZone(other);
        }
    }

    void EnteredMagnetZone(Collider2D other)
    {
        if (_isCollected)
            return;

        _collectedCar = other.transform.parent.GetComponent<CarScript>();

        _isCollected = true;

        StartCoroutine(MoveToTarget());
    }

    IEnumerator MoveToTarget()
    {
        Vector2 velocity = Vector2.zero;

        while (Vector2.Distance(Rigidbody.position, _collectedCar.MovementController.Rigidbody.position) > 2f)
        {
            Rigidbody.position = Vector2.SmoothDamp(Rigidbody.position, _collectedCar.MovementController.Rigidbody.position, ref velocity, DampTime);

            yield return new WaitForFixedUpdate();
        }

        _collectedCar.AddScore(WeaponDropManager.Instance.WeaponDropBonusPoint, false);

        _collectedCar.WeaponSystemController.ActivateWeaponSystem(WeaponType, AmmoCount);

        if (_collectedCar.IsPlayerCar)
            _collectedCar.FireOnCollectedWeapon(WeaponType);

        IsAcquired = true;
    }
}
