using UnityEngine;
using System.Collections;

public class CoinCarAnimationController : CarAnimationControllerBase
{
    const string IDLE_SPRITE_NAME = "dollar_car_b";
    const string ROTATE_SPRITENAME = "dollar_car_a";

    protected override void InitSpriteIDs()
    {

        _idleSpriteID = CarSprite.Collection.GetSpriteIdByName(IDLE_SPRITE_NAME);
        _rotateSpriteID = CarSprite.Collection.GetSpriteIdByName(ROTATE_SPRITENAME);

    }
}
