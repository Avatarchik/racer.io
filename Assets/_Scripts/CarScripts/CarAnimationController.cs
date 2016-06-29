using UnityEngine;
using System.Collections;

public enum CarAnimEnum
{
    None,
    Idle,
    Rotate,
}

public class CarAnimationController : MonoBehaviour
{
    public CarScript MyCar;
    public MeshRenderer CarRenderer;
    public tk2dSpriteAnimator SpriteAnimator;

    const string IDLE_ANIM_NAME = "Idle";
    const string ROTATE_ANIM_NAME = "Rotate";

    string _idleAnimName;
    string _rotateAnimName;

    bool _canChangeAnim;

    CarAnimEnum _curAnimEnum;

    IEnumerator _waitForAnimFinishRoutine;

    public void InitAnimationController()
    {
        _canChangeAnim = true;

        CarRenderer.enabled = true;

        InitAnimNames();
    }

    void InitAnimNames()
    {        
        _idleAnimName = IDLE_ANIM_NAME + "_" + MyCar.CarType.ToString() + "_" + MyCar.CarColor.ToString();
        _rotateAnimName = ROTATE_ANIM_NAME + "_" + MyCar.CarType.ToString() + "_" + MyCar.CarColor.ToString();
    }

    public void PlayAnim(CarAnimEnum animEnum)
    {
        if (_curAnimEnum == animEnum)
            return;
        
        _curAnimEnum = animEnum;

        switch (animEnum)
        {
            case CarAnimEnum.Idle:
                SpriteAnimator.Play(_idleAnimName);
                break;
            case CarAnimEnum.Rotate:
                SpriteAnimator.Play(_rotateAnimName);
                break;
        }
        
    }
}
