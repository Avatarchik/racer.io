using UnityEngine;
using System.Collections;

public interface IDamagable
{
    void InitCurHealth();

    void InitDamagable();

    IDamagable GetDamagableInterface();

    void TakeDamage(int damageAmount, CarScript car);

    void Die(CarScript car);
}
