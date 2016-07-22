using UnityEngine;
using System.Collections;


public class CombatCarAnimationController : CarAnimationControllerBase
{
    const string IDLE_SPRITE_NAME = "_b";
    const string ROTATE_SPRITE_NAME = "_a";

    protected override void InitSpriteIDs()
    {        
        string idleSpriteName = MyCar.CarType.ToString() + "_" + MyCar.CarColor.ToString() + IDLE_SPRITE_NAME;
        idleSpriteName = idleSpriteName.ToLower();

        string rotateSpriteName = MyCar.CarType.ToString() + "_" + MyCar.CarColor.ToString() + ROTATE_SPRITE_NAME;
        rotateSpriteName = rotateSpriteName.ToLower();

        _idleSpriteID = CarSprite.Collection.GetSpriteIdByName(idleSpriteName);
        _rotateSpriteID = CarSprite.Collection.GetSpriteIdByName(rotateSpriteName);

    }
}
