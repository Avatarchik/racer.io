using UnityEngine;
using System.Collections;

public enum AbandonSide
{
    Left,
    Right,
    Top,
    Bottom
}

public class AbandonAreaController : MonoBehaviour
{
    public AbandonSide AbandonSide;
    public SpriteRenderer Sprite;
    public Transform StartingPoint, EndPoint;

    [HideInInspector]
    public float AlphaAmount = 0f;

    public void FixedUpdateFrame()
    {
        Transform playerCar = SinglePlayerArenaCarManager.Instance.GetPlayerCar();

        if (playerCar != null)
        {
            switch (AbandonSide)
            {
                case AbandonSide.Right:
                    if (playerCar.position.x > StartingPoint.position.x && playerCar.position.x < EndPoint.position.x)
                        AlphaAmount = (playerCar.position.x - StartingPoint.position.x) / (EndPoint.position.x - StartingPoint.position.x);
                    break;
                case AbandonSide.Left:
                    if (playerCar.position.x < StartingPoint.position.x && playerCar.position.x > EndPoint.position.x)
                        AlphaAmount = (playerCar.position.x - StartingPoint.position.x) / (EndPoint.position.x - StartingPoint.position.x);
                    break;
                case AbandonSide.Top:
                    if (playerCar.position.y > StartingPoint.position.y && playerCar.position.y < EndPoint.position.y)
                        AlphaAmount = (playerCar.position.y - StartingPoint.position.y) / (EndPoint.position.y - StartingPoint.position.y);
                    break;
                case AbandonSide.Bottom:
                    if (playerCar.position.y < StartingPoint.position.y && playerCar.position.y > EndPoint.position.y)
                        AlphaAmount = (playerCar.position.y - StartingPoint.position.y) / (EndPoint.position.y - StartingPoint.position.y);
                    break;
                default:
                    AlphaAmount = 0f;
                    break;
            }
        }
    }

    public void Reset()
    {
        AlphaAmount = 0f;
    }
}
