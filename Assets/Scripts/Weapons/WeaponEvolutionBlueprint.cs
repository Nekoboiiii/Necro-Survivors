using UnityEngine;

[System.Obsolete("Will be replaced by the new System")]
[CreateAssetMenu(fileName = "WeaponEvolutionBlueprint", menuName = "Scriptable Objects/WeaponEvolutionBlueprint")]
public class WeaponEvolutionBlueprint : ScriptableObject
{
    public WeaponScriptableObject baseWeaponData;
    public PassivItemScriptableObject catalystPassiveItenData;
    public WeaponScriptableObject evolvedWeaponData;
    public GameObject evolvedWeapon;
}
