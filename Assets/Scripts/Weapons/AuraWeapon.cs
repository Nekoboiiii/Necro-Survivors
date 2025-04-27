using UnityEngine;

public class AuraWeapon : Weapon
{
    protected Aura currentAura;

    protected override void Update()
    {
        // Intentionally empty - override base behavior
    } 

    public override void OnEquip()
    {
        // Try to replace the aura the weapon has with a new one
        if (currentStats.auraPrefab)
        {
            if (currentAura)
            {
                Destroy(currentAura.gameObject);
            }
            currentAura = Instantiate(currentStats.auraPrefab, transform);
            currentAura.weapon = this;
            currentAura.owner = owner;
        }
    }

    public override void OnUnequip()
    {
        if (currentAura)
        {
            Destroy(currentAura.gameObject);
        }
    }

    public override bool DoLevelUp()
    {
        if (!base.DoLevelUp())
        {
            return false;
        }

        // If there is an aura attached to this weapon we update the aura
        if (currentAura)
        {
            currentAura.transform.localScale = new Vector3(currentStats.area, currentStats.area, currentStats.area);
        }
        
        return true;
    }
}