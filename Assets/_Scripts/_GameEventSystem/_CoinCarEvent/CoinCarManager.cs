using UnityEngine;
using System.Collections;

public class CoinCarManager : MonoBehaviour
{
    static CoinCarManager _instance;

    public static CoinCarManager Instance{ get { return _instance; } }

    public CoinCar CoinCar;

    public float CoinCarSpawnInterval;

    bool _canSpawnCoinCar;

    void Awake()
    {
        _instance = this;
    }

    void OnDestroy()
    {
        _instance = null;
    }

    public void StartSpawnProgress()
    {
        _canSpawnCoinCar = true;

        SpawnCoinCar();
    }

    public void StopSpawnProgress()
    {
        _canSpawnCoinCar = false;
    }

    void SpawnCoinCar()
    {
        Debug.Log("spawn coin car");
        Vector2 spawnPos = GameArea.Instance.GetRandomPosInGameArea();

        CoinCar.SetInitPosition(spawnPos);

        CoinCar.SetInitHealth(false);

        CoinCar.Activate(true);

        CoinCar.CarAI.ActivateAI();

        GeneralCarManager.Instance.RegisterToActiveCarList(CoinCar);
    }

    public void CoinCarKilled()
    {
        CoinCar.Deactivate();
            
        StartCoroutine(WaitForSpawnCoinCar());
    }

    IEnumerator WaitForSpawnCoinCar()
    {
        if (!_canSpawnCoinCar)
            yield break;

        yield return new WaitForSeconds(CoinCarSpawnInterval);

        if (!_canSpawnCoinCar)
            yield break;

        SpawnCoinCar();
    }

}
