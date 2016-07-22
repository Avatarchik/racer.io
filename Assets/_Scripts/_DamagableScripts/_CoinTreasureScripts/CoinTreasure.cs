using UnityEngine;
using System.Collections;

public class CoinTreasure : SpawnableBase, IDamagable
{
    public AudioSource DieSound;
    
    public int MaxHealth;

    int _curHealth;

    public int CoinAmount;

    bool _canTakeDamage;

    IDamagable _damagableInterface;

    public override void Activate(Vector3 spawnPos)
    {
        InitCurHealth();
        
        _canTakeDamage = true;
        
        base.Activate(spawnPos);
    }

    #region IDamagable implementation

    public void InitCurHealth()
    {
        _curHealth = MaxHealth;
    }

    public void InitDamagable()
    {
        _damagableInterface = gameObject.GetInterface<IDamagable>();
    }

    public IDamagable GetDamagableInterface()
    {
        return _damagableInterface;
    }

    public void TakeDamage(int damageAmount, CombatCarScript car)
    {
        if (!_canTakeDamage)
            return;
        
        _curHealth -= damageAmount;

        if (_curHealth <= 0)
        {
            _curHealth = 0;

            Die(car);
        }
    }

    public void Die(CombatCarScript car)
    {
        _canTakeDamage = false;
        
        if (car.ControllerType != CarControllerType.Player)
            return;

        DieSound.Play();

        StartCoroutine(Utilities.WaitForSoundFinish(DieSound, Deactivate));

        CurrencyManager.Instance.GainCurrency(CurrencyType.Coin, CoinAmount);
    }

    #endregion
    
}
