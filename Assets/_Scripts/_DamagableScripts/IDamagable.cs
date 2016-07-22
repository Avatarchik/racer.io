using UnityEngine;
using System.Collections;

public interface IDamagable
{
    void InitCurHealth();

    void InitDamagable();

    IDamagable GetDamagableInterface();

    void TakeDamage(int damageAmount, CombatCarScript car);

    void Die(CombatCarScript car);
}
