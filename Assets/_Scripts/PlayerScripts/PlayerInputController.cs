using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;

public class PlayerInputController : MonoBehaviour
{
    public CombatCarScript MyCar;
    public CombatCarMovementController MovementController;

    Vector3 _inputDirection;

    const string HOR_AXIS = "Horizontal";
    const string VER_AXIS = "Vertical";

    bool _canGetInput;
    IEnumerator _restrictInputRoutine;

    bool _isFireButtonPressed;
    bool _isLeftButtonPressed;
    bool _isRightButtonPressed;

    public void ActivateInputController()
    {
        _isFireButtonPressed = false;
        _isLeftButtonPressed = false;
        _isRightButtonPressed = false;
        
        _canGetInput = true;

        StartListeningEvents();
    }

    public void DeactivateInputController()
    {
        FinishListeningEvents();
    }

    void StartListeningEvents()
    {
        InputHandler.OnFireButtonPressed += OnFireButtonPressed;
        InputHandler.OnFireButtonReleased += OnFireButtonReleased;
        InputHandler.OnLeftButtonPressed += OnLeftButtonPressed;
        InputHandler.OnLeftButtonReleased += OnLeftButtonReleased;
        InputHandler.OnRightButtonPressed += OnRightButtonPressed;
        InputHandler.OnRightButtonReleased += OnRightButtonReleased;
    }

    void FinishListeningEvents()
    {
        InputHandler.OnFireButtonPressed -= OnFireButtonPressed;
        InputHandler.OnFireButtonReleased -= OnFireButtonReleased;
        InputHandler.OnLeftButtonPressed -= OnLeftButtonPressed;
        InputHandler.OnLeftButtonReleased -= OnLeftButtonReleased;
        InputHandler.OnRightButtonPressed -= OnRightButtonPressed;
        InputHandler.OnRightButtonReleased -= OnRightButtonReleased;
    }

    void OnFireButtonPressed()
    {
        _isFireButtonPressed = true;
    }

    void OnFireButtonReleased()
    {
        _isFireButtonPressed = false;
    }

    void OnLeftButtonPressed()
    {
        _isLeftButtonPressed = true;
    }

    void OnLeftButtonReleased()
    {
        _isLeftButtonPressed = false;
    }

    void OnRightButtonPressed()
    {
        _isRightButtonPressed = true;
    }

    void OnRightButtonReleased()
    {
        _isRightButtonPressed = false;
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

        #if UNITY_EDITOR
        CheckKeyboard();
        #endif

        CheckSteering();
        CheckFiring();

        ApplyInput();
    }


    void CheckKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _isFireButtonPressed = true;
        else if (Input.GetKeyUp(KeyCode.Space))
            _isFireButtonPressed = false;

        if (Input.GetKeyDown(KeyCode.A))
            _isLeftButtonPressed = true;
        else if (Input.GetKeyUp(KeyCode.A))
            _isLeftButtonPressed = false;

        if (Input.GetKeyDown(KeyCode.D))
            _isRightButtonPressed = true;
        else if (Input.GetKeyUp(KeyCode.D))
            _isRightButtonPressed = false;
    }

    void CheckSteering()
    {
        if (_isLeftButtonPressed)
            _inputDirection = new Vector2(-1, 0);
        else if (_isRightButtonPressed)
            _inputDirection = new Vector2(1, 0);
    }

    void CheckFiring()
    {
        if (!_isFireButtonPressed)
            return;

        MyCar.WeaponController.Fire();
    }

    void ApplyInput()
    {
        Vector2 targetPos = transform.position;

        _inputDirection.Normalize();

        targetPos += (Vector2)_inputDirection * CarSettings.Instance.MaxSpeed;

        MovementController.SeekByInput(_inputDirection);

        _inputDirection = Vector2.zero;
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

        _isLeftButtonPressed = false;
        _isRightButtonPressed = false;
    }
}