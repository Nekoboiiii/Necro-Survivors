using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

/// <summary>
/// Componet to be attached to all Weapon prefabs. The weapon prefab works together with the WeaponData
/// ScriptableObjects to manage and run the behaviours of all weapons in the game.
/// </summary>

public abstract class Weapon : Item
{
    [System.Serializable]
    public struct Stats
    {
        public string name, description;

        [Header("Visuals")]
        public Projectile projectilePrefab; // If attached a projectile will spawn every time the weapon cooldown ends
        public Aura auraPrefab;  // if attached an aura will spawn when when weapon is equipped
        public ParticleSystem hitEffect;
        public Rect spawnVariance;

        [Header("Values")]
        public float lifespan; // If 0 it will last forever
        public float damage, damageVariance, area, speed, cooldown, projectileInterval, knockback;
        public int number, piercing, maxInstances;

        // Allows us to use the + operator to add 2 Stats together
        // Very importan later when we want to increase our weapon stats
        public static Stats operator +(Stats s1, Stats s2)
        {
            Stats result = new Stats();
            result.name = s2.name ?? s1.name;
            result.description = s2.description ?? s1.description;
            result.projectilePrefab = s2.projectilePrefab ?? s1.projectilePrefab;
            result.auraPrefab = s2.auraPrefab ?? s1.auraPrefab;
            result.hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect;
            result.spawnVariance = s2.spawnVariance;
            result.lifespan = s2.lifespan + s1.lifespan;
            result.damage = s2.damage + s1.damage;
            result.damageVariance = s2.damageVariance + s1.damageVariance;
            result.area = s2.area + s1.area;
            result.speed = s2.speed + s1.speed;
            result.cooldown= s2.cooldown + s1.cooldown;
            result.number = s2.number + s1.number;
            result.piercing = s2.piercing + s1.piercing;
            result.projectileInterval = s2.projectileInterval + s1.projectileInterval;
            result.knockback = s2.knockback + s1.knockback;
            return result;
        }

        //Get Damage dealt
        public float GetDamage()
        {
            return damage + Random.Range(0, damageVariance);
        }
    }
        protected Stats currentStats;
        public WeaponData data;
        protected float currentCooldown;
        protected PlayerMovement movement; // Was misspelled as "movment"

        // For dynamically created weapons, call initialise to set everything up
        public virtual void Initialise(WeaponData data)
        {
            base.Initialise(data);
            this.data = data;
            currentStats = data.baseStats;
            movement = GetComponentInParent<PlayerMovement>();
            currentCooldown = currentStats.cooldown;
        }

        protected virtual void Awake()
        {
            // Assign the stats early, as it will be used by other scripts later on
            if (data)
            {
                currentStats = data.baseStats;
            }
        }

        protected virtual void Start()
        {
            // Dont ininitalise the weapon if the weapon is not assign
            if (data)
            {
                Initialise(data);
            }
        }

        protected virtual void Update()
        {
            currentCooldown -= Time.deltaTime;
            if(currentCooldown <= 0f)  // Once the cooldown becomes 0, attack
            {
                Attack(currentStats.number);
            }
        }

        // Levels up the weapon by 1 and calculates the corresponding stats
        public override bool DoLevelUp()
        {
            if (!base.DoLevelUp())
            {
                return false;
            }
            
            // Prevent level up if we are already at max level
            if (!CanLevelUp())
            {
                Debug.LogWarning(string.Format("Cannot level up {0} to level {1}, max level of {2} already reached",name, currentLevel, data.maxLevel));
                return false;
            }

            // Otherwise, add stats of the next level to our weapon
            currentStats += data.GetLevelData(++currentLevel);
            return true;

        }
        // Lets us check whether this weapon can attack at this current moment
        public virtual bool CanAttack()
        {
            return currentCooldown <= 0;
        }
        // Performs an attack with the weapon
        // Returns true if the attack was sucsessful
        // This doesnt do anything. We have to override this at the child class to add a behaviour
        protected virtual bool Attack(int attackCount = 1)
        {
            if (CanAttack())
            {
                currentCooldown += currentStats.cooldown;
                return true;
            }
            return false;
        }

        // Gets the amount of damage that the weapon is supposed to deal
        // Factoring in the weapons stats (inculding damage variance)
        // as well as the characters Might stat
        public virtual float GetDamage()
        {
            return currentStats.GetDamage() * owner.CurrentMight;
        }

        // For retrieving the weapons stats

        public virtual Stats GetStats()
        {
            return currentStats;
        }
    
}
