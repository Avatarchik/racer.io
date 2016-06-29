using UnityEngine;
using System.Collections;

public class InGameAreaDrawer : MonoBehaviour
{
    public BoxCollider2D GameAreaCollider, EffectiveGameAreaCollider, WarningArea, AbandonArea;

    void OnDrawGizmos()
    {
        if (EffectiveGameAreaCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(EffectiveGameAreaCollider.offset, EffectiveGameAreaCollider.size);
        }

        if (GameAreaCollider != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(GameAreaCollider.offset, GameAreaCollider.size);
        }

        if (WarningArea != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(WarningArea.offset, WarningArea.size);
        }

        if (AbandonArea != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(AbandonArea.offset, AbandonArea.size);
        }
    }
}
