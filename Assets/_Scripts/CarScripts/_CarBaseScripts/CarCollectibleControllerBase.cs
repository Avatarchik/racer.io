using UnityEngine;
using System.Collections;

public class CarCollectibleControllerBase : MonoBehaviour
{
    public CarBase MyCar;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == (int)LayerEnum.Collectible)
        {
            CollectedCollectible(other);
        }
    }

    void CollectedCollectible(Collider2D other)
    {
        CollectibleBase collectible = other.GetComponent<CollectibleBase>();

        collectible.Collected(MyCar);
    }
}
