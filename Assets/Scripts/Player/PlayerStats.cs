using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    CharacterScriptableObject characterData;

    // Current Stats
    float currentHealth;
    float currentRecovery;
    float currentMoveSpeed;
    float currentMight;
    float currentProjectileSpeed;
    float currentMagnet;

    #region Current Stats
    // Getters and Setters for the current stats
    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            // Check if the value has changed
            if(currentHealth != value)
            {
                currentHealth = value;
                // Update the real time value of the stat
                // Add any additional logic here that needs to be executed when the value changes
                if(GameManager.instance != null)
                {
                        GameManager.instance.currentHealthDisplay.text = "Health: " + currentHealth;
                }
            }
        }
    }
    public float CurrentRecovery
    {
        get { return currentRecovery; }
        set
        {
            // Check if the value has changed
            if(currentRecovery != value)
            {
                currentRecovery = value;
                // Update the real time value of the stat
                // Add any additional logic here that needs to be executed when the value changes
                if(GameManager.instance != null)
                {
                        GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery;
                }
            }
        }
    }
    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set
        {
            // Check if the value has changed
            if(currentMoveSpeed != value)
            {
                currentMoveSpeed = value;
                // Update the real time value of the stat
                // Add any additional logic here that needs to be executed when the value changes
                if(GameManager.instance != null)
                {
                        GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed;
                }
            }
        }
    }
    public float CurrentMight
    {
        get { return currentMight; }
        set
        {
            // Check if the value has changed
            if(currentMight != value)
            {
                currentMight = value;
                // Update the real time value of the stat
                // Add any additional logic here that needs to be executed when the value changes
                if(GameManager.instance != null)
                {
                        GameManager.instance.currentMightDisplay.text = "Might: " + currentMight;
                }
            }
        }
    }
    public float CurrentProjectileSpeed
    {
        get { return currentProjectileSpeed; }
        set
        {
            // Check if the value has changed
            if(currentProjectileSpeed != value)
            {
                currentProjectileSpeed = value;
                // Update the real time value of the stat
                // Add any additional logic here that needs to be executed when the value changes
                if(GameManager.instance != null)
                {
                        GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed;
                }
            }
        }
    }
    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set
        {
            // Check if the value has changed
            if(currentMagnet != value)
            {
                currentMagnet = value;
                // Update the real time value of the stat
                // Add any additional logic here that needs to be executed when the value changes
                if(GameManager.instance != null)
                {
                        GameManager.instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet;
                }
            }
        }
    }
    #endregion
    // Experience and level of the Player
    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    // I-Frames
    [Header("I-Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;

    public int weaponIndex;
    public int passivItemIndex;

    public List<LevelRange> levelRange;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TextMeshProUGUI levelText;

    [Header("Visuals")]
    public ParticleSystem damageEffect;

    InventoryManager inventory;

    void Awake()
    {
        characterData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySingleton();

        inventory = FindFirstObjectByType<InventoryManager>();

        // Assign the Variables
        CurrentHealth = characterData.MaxHealth;
        CurrentRecovery = characterData.Recovery;
        CurrentMoveSpeed = characterData.MoveSpeed;
        CurrentMight = characterData.Might;
        CurrentProjectileSpeed = characterData.ProjectileSpeed;
        CurrentMagnet = characterData.Magnet;

        //Spawn the starting Weapon
        SpawnWeapon(characterData.StartingWeapon);
    }

    void Start()
    {
        // Initialize the experience cap as the first experience cap increase
        experienceCap = levelRange[0].experienceCapIncrease;

        // Set the current stats display
            GameManager.instance.currentHealthDisplay.text = "Health: " + CurrentHealth;
            GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + CurrentRecovery;
            GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + CurrentMoveSpeed;
            GameManager.instance.currentMightDisplay.text = "Might: " + CurrentMight;
            GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + CurrentProjectileSpeed;
            GameManager.instance.currentMagnetDisplay.text = "Magnet: " + CurrentMagnet;

            GameManager.instance.AssignChosenCharacterUI(characterData);
            UpdateHealthBar();
            UpdateExpBar();
            UpdateLevelText();
    }

    void Update()
    {
        if(invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        // If the invincibility timer has reached 0, set the invincibility state to false
        else if (isInvincible)
        {
            isInvincible = false;
        }
        Recover();
    }

    public void IncreaseExperience(int Amount)
    {
        experience += Amount;
        LevelUpChecker();
        UpdateExpBar();
    }

    void LevelUpChecker()
    {
        if(experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach (LevelRange range in levelRange)
            {
                if(level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;

            UpdateLevelText();

            GameManager.instance.StartLevelUp(); // Start the level up process
                
        }
    }

    void UpdateExpBar()
    {
        // Update exp bar fill amount
        expBar.fillAmount = (float)experience /experienceCap;
    }

    void UpdateLevelText()
    {
        // Update level text
        levelText.text = "LV" + level.ToString();
    }

    public void TakeDamage(float dmg)
    {
        // If the player is not invincible, take damage and start the invincibility timer
        if(!isInvincible)
        {
            CurrentHealth -= dmg;

            // If there ist a damage effect assigned, play it
            if (damageEffect)
            {
                ParticleSystem instantiatedEffect = Instantiate(damageEffect, transform.position, Quaternion.identity);
                instantiatedEffect.transform.SetParent(transform); // Make the particle system a child of the player
            }

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

            if(CurrentHealth <= 0)
            {
                Kill();
            }

            UpdateHealthBar();
        }
       
        
    }

    void UpdateHealthBar()
    {
        // Update the health bar
        healthBar.fillAmount = currentHealth /characterData.MaxHealth;
    }

    public void Kill()
    {
       Debug.Log("Player is Dead");
       if(!GameManager.instance.isGameOver)
       {
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.AssignChosenWeaponsAndPassivItemsUI(inventory.weaponUISlots, inventory.passivItemUISlots);
            GameManager.instance.GameOver();
       }
    }

    public void RestoreHealth(float amount)
    {
        // Only healthe player if their health is below the max health
        if(CurrentHealth < characterData.MaxHealth)
        {
            CurrentHealth += amount;

            // Makes sure the player does not go over the max health
            if(CurrentHealth > characterData.MaxHealth)
            {
                CurrentHealth = characterData.MaxHealth;
            }
            UpdateHealthBar();
        }
        
    }

    void Recover()
    {
        // Only recover the player if their health is below the max health
        if(CurrentHealth < characterData.MaxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;

            // Makes sure the player does not go over the max health
            if(CurrentHealth > characterData.MaxHealth)
            {
                CurrentHealth = characterData.MaxHealth;
            }
            UpdateHealthBar();
        }
    }

    public void SpawnWeapon(GameObject weapon)
    {
        // Check if the weapon index is full, and return if it is
        if (weaponIndex >= inventory.weaponSlots.Count -1)
        {
            Debug.LogError("Weapon Inventory is full. Cannot spawn more weapons.");
            return;
        }
        
        // Spawn the starting Weapon
        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform);
        inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>()); // Add the weapon to the inventory

        weaponIndex++;
    }

    public void SpawnPassivItem(GameObject weapon)
    {
        // Check if the weapon index is full, and return if it is
        if (weaponIndex >= inventory.passivItemSlots.Count -1)
        {
            Debug.LogError("PassivItem Inventory is full. Cannot spawn more weapons.");
            return;
        }
        
        // Spawn the starting passiv item
        GameObject spawnedPassivItem = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedPassivItem.transform.SetParent(transform);
        inventory.AddPassivItem(passivItemIndex, spawnedPassivItem.GetComponent<PassivItem>()); // Add the weapon to the inventory

        passivItemIndex++;
    }
    
}
