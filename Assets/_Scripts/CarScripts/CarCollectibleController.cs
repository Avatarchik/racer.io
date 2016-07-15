using UnityEngine;
using System.Collections;

public class CarCollectibleController : MonoBehaviour
{
    public CarScript MyCar;

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
