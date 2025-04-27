using UnityEngine;

/// <summary>
/// A GameObject that is spawned as an effect of a weapon firing, e. g. projectiles, auras, pulses
/// </summary>

public abstract class WeaponEffect : MonoBehaviour
{
    public Weapon weapon;
    public PlayerStats owner;

    protected virtual float GetDamage()
    {
        return weapon.GetDamage();
    }

}