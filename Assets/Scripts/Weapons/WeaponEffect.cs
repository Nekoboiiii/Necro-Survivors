using UnityEngine;

/// <summary>
/// A GameObject that is spawned as an effect of a weapon firing, e. g. projectiles, auras, pulses
/// </summary>

public abstract class WeaponEffects : MonoBehaviour
{
    [HideInInspector]
    public PlayerStats owner;
    [HideInInspector]
    public WeaponEffects weapon;

    public float GetDamage()
    {
        return weapon.GetDamage();
    }

}