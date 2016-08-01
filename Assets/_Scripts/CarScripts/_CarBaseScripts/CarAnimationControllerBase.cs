using UnityEngine;
using System.Collections;


public enum CarAnimEnum
{
    None,
    Idle,
    Rotate,
}

public class CarAnimationControllerBase : MonoBehaviour
{
    public CarBase MyCar;
    public MeshRenderer CarRenderer;
    public tk2dSprite CarSprite;

    protected int _idleSpriteID;
    protected int _rotateSpriteID;

    bool _canChangeAnim;

    CarAnimEnum _curAnimEnum;

    public virtual void InitAnimationController()
    {
        _curAnimEnum = CarAnimEnum.None;
        
        _canChangeAnim = true;

        CarRenderer.enabled = true;

        InitSpriteIDs();

        PlayAnim(CarAnimEnum.Idle);
    }

    protected virtual void InitSpriteIDs()
    {
        
    }

    public void PlayAnim(CarAnimEnum animEnum)
    {
        if (_curAnimEnum == animEnum)
            return;

        _curAnimEnum = animEnum;

        switch (animEnum)
        {
            case CarAnimEnum.Idle:
                CarSprite.SetSprite(_idleSpriteID);
                break;
            case CarAnimEnum.Rotate:
                CarSprite.SetSprite(_rotateSpriteID);
                break;
        }

    }
}
