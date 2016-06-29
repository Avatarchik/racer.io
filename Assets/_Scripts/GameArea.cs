using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameArea : MonoBehaviour
{
    static GameArea _instance;

    public static GameArea Instance { get { return _instance; } }

    public BoxCollider2D GameAreaBorders, EffectiveGameAreaBorders, WarningAreaBorders, AbandonAreaBorders;
    public List<AbandonAreaController> AbandonAreaControllerList;

    float _alphaAmount;

    void Awake()
    {
        _instance = this;
    }

    public void ResetAbondonAreaControllerList()
    {
        AbandonAreaControllerList.ForEach(c => c.Reset());
    }

    public bool IsInGameArea(Vector2 position)
    {
        return GameAreaBorders.bounds.Contains(position);
    }

    public bool IsInEffectiveArea(Vector2 position)
    {
        return EffectiveGameAreaBorders.bounds.Contains(position);
    }

    public bool IsInWarningArea(Vector2 position)
    {
        return WarningAreaBorders.bounds.Contains(position);
    }

    public bool IsInAbandonArea(Vector2 position)
    {
        return AbandonAreaBorders.bounds.Contains(position);
    }

    public Vector2 GetRandomPosInGameArea()
    {
        Vector2 randPos;

        randPos.x = Utilities.NextFloat(GameAreaBorders.bounds.min.x, GameAreaBorders.bounds.max.x);
        randPos.y = Utilities.NextFloat(GameAreaBorders.bounds.min.y, GameAreaBorders.bounds.max.y);

        return randPos;
    }

    public Vector2 GetRandomPosInEffectiveGameArea()
    {
        Vector2 randPos;

        randPos.x = Utilities.NextFloat(EffectiveGameAreaBorders.bounds.min.x, EffectiveGameAreaBorders.bounds.max.x);
        randPos.y = Utilities.NextFloat(EffectiveGameAreaBorders.bounds.min.y, EffectiveGameAreaBorders.bounds.max.y);

        return randPos;
    }

    public float GetBorderPos(GameAreaSideEnum side)
    {
        switch (side)
        {
            case GameAreaSideEnum.MaxX:
                return GameAreaBorders.bounds.max.x;
            case GameAreaSideEnum.MaxY:
                return GameAreaBorders.bounds.max.y;
            case GameAreaSideEnum.MinX:
                return GameAreaBorders.bounds.min.x;
            case GameAreaSideEnum.MinY:
                return GameAreaBorders.bounds.min.y;
            default:
                return 0f;
        }
    }

    public void FixedUpdateFrame()
    {
        AbandonAreaControllerList.ForEach(c => c.FixedUpdateFrame());

        AbandonAreaControllerList = AbandonAreaControllerList.OrderByDescending(c => c.AlphaAmount).ToList();

        _alphaAmount = AbandonAreaControllerList[0].AlphaAmount;

        AbandonAreaControllerList.ForEach(c => c.Sprite.color = new Color(c.Sprite.color.r, c.Sprite.color.g, c.Sprite.color.b, _alphaAmount));
    }
}