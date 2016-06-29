using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;

public class PlayerInputController : MonoBehaviour
{
    public CarScript MyCar;
    public CarMovementController MovementController;

    Vector3 _inputDirection;

    const string HOR_AXIS = "Horizontal";
    const string VER_AXIS = "Vertical";

    bool _canGetInput;
    IEnumerator _restrictInputRoutine;

    public void ActivateInputController()
    {
        _canGetInput = true;
    }

    public void SetInputDirection(Vector2 direction)
    {
        _inputDirection = direction;
    }

    public void FixedUpdateFrame()
    {
        if (MyCar.ControllerType == CarControllerType.NPC)
            return;

        if (!_canGetInput)
            _inputDirection = Vector2.zero;

        if (MyCar.IsPlayerCar && _canGetInput)
            CheckInput();

        ApplyInput();
    }

    void CheckInput()
    {
        //PlayerCarFireController.Instance.FireCarWeapon();
        
        CheckDefaultMovement();
    }

    void CheckDefaultMovement()
    {
        _inputDirection = Vector2.zero;
        
        if (Input.GetKey(KeyCode.A))
            _inputDirection = new Vector2(-1, 0);
        else if (Input.GetKey(KeyCode.D))
            _inputDirection = new Vector2(1, 0);
    }

    void ApplyInput()
    {
        Vector2 targetPos = transform.position;

        _inputDirection.Normalize();

        targetPos += (Vector2)_inputDirection * CarSettings.Instance.MaxSpeed;

        MovementController.SeekByInput(_inputDirection);

    }

    Vector2 GetTargetPos()
    {
        return Vector2.zero;
    }

    public void RestrictPlayerInput(float duration)
    {
        if (_restrictInputRoutine != null)
            StopCoroutine(_restrictInputRoutine);

        _restrictInputRoutine = RestrictInputProgress(duration);
        StartCoroutine(_restrictInputRoutine);
    }

    IEnumerator RestrictInputProgress(float duration)
    {
        _canGetInput = false;

        yield return new WaitForSeconds(duration);

        _canGetInput = true;
    }
}