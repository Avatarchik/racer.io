using UnityEngine;
using System.Collections;

public interface ICollectible
{
    void Collected();

    CollectibleTypeEnum GetCollectibleType();
}
