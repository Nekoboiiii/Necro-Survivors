using UnityEngine;

[System.Obsolete("Will be replaced by the new System")]
public class RitualController : WeaponController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();  
    }

    // Update is called once per frame
    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedRitual = Instantiate(weaponData.Prefab);
        spawnedRitual.transform.position = transform.position;
        spawnedRitual.transform.parent = transform;
    }
}
