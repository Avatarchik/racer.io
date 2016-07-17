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
    public tk2dSprite CarSprite;

    const string IDLE_SPRITE_NAME = "_b";
    const string ROTATE_SPRITE_NAME = "_a";

    int _idleSpriteID;
    int _rotateSpriteID;

    bool _canChangeAnim;

    CarAnimEnum _curAnimEnum;

    public void InitAnimationController()
    {
        _canChangeAnim = true;

        CarRenderer.enabled = true;

        InitSpriteIDs();

        PlayAnim(CarAnimEnum.Idle);
    }

    void InitSpriteIDs()
    {        
        string idleSpriteName = MyCar.CarType.ToString() + "_" + MyCar.CarColor.ToString() + IDLE_SPRITE_NAME;
        idleSpriteName = idleSpriteName.ToLower();

        string rotateSpriteName = MyCar.CarType.ToString() + "_" + MyCar.CarColor.ToString() + ROTATE_SPRITE_NAME;
        rotateSpriteName = rotateSpriteName.ToLower();

        _idleSpriteID = CarSprite.Collection.GetSpriteIdByName(idleSpriteName);
        _rotateSpriteID = CarSprite.Collection.GetSpriteIdByName(rotateSpriteName);

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
