using System.Collections.Generic;
using UnityEngine;

public class Aura : WeaponEffect
{
    Dictionary<EnemyStats, float> affectedTargets = new Dictionary<EnemyStats, float>();
    List<EnemyStats> targetToUnaffect = new List<EnemyStats>();

    void Update()
    {
        Dictionary<EnemyStats, float> affectedTargsCopy = new Dictionary<EnemyStats, float>(affectedTargets);

        // Loop through every target affected by the aura and reduce the cooldown
        // of the aura for it. If the cooldown reaches 0, deal damage to it
        foreach (KeyValuePair<EnemyStats, float> pair in affectedTargsCopy)
        {
            float cooldown = pair.Value - Time.deltaTime;
            affectedTargets[pair.Key] = cooldown;
            
            if (cooldown <= 0)
            {
                if (targetToUnaffect.Contains(pair.Key))
                {
                    // If the target is marked for removal remove it
                    affectedTargets.Remove(pair.Key);
                    targetToUnaffect.Remove(pair.Key);
                }
                else
                {
                    // Reset the cooldown and deal damage
                    Weapon.Stats stats = weapon.GetStats();
                    affectedTargets[pair.Key] = stats.cooldown;
                    pair.Key.TakeDamage(GetDamage(), transform.position, stats.knockback);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyStats es = other.GetComponent<EnemyStats>();
        if (es != null)
        {
            // If the target is not yet affected by this aura add it to our list of affected targets
            if (!affectedTargets.ContainsKey(es))
            {
                // Always starts with an interval of 0 so that it will get damaged in the next Update() tick
                affectedTargets.Add(es, 0);
            }
            else
            {
                if (targetToUnaffect.Contains(es))
                {
                    targetToUnaffect.Remove(es);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Do not directly remove the target upon leaving because we still have to track their cooldowns
        EnemyStats es = other.GetComponent<EnemyStats>();
        if (es != null && affectedTargets.ContainsKey(es))
        {
            targetToUnaffect.Add(es);
        }
    }
}