using System.Collections.Generic;
using System;
using TMPro;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;

    float health;

    #region Current Stats
    public float CurrentHealth
    {
        
        get {return health; }

        set
        {
            // Check if the value has changed
            if(health != value)
            {
                health = value;
                if(GameManager.instance != null)
                {
                        GameManager.instance.currentHealthDisplay.text = string.Format("Health {0} / {1}", health, actualStats.maxHealth);
                }
            }
        }
    }
    public float MaxHealth
    {
        
        get {return actualStats.maxHealth; }

        set
        {
            // Check if the value has changed
            if(actualStats.maxHealth != value)
            {
                actualStats.maxHealth = value;
                if(GameManager.instance != null)
                {
                        GameManager.instance.currentHealthDisplay.text = string.Format("Health {0} / {1}", health, actualStats.maxHealth);
                }
            }
        }
    }
    public float CurrentRecovery
    {
        get { return Recovery; }
        set { Recovery = value; }
    }

    public float Recovery
    {
        get {return actualStats.recovery; }
        set
        {
            // Check if the value has changed
            if(actualStats.recovery != value)
            {
                actualStats.recovery = value;
                // Update the real time value of the stat
                // Add any additional logic here that needs to be executed when the value changes
                if(GameManager.instance != null)
                {
                        GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + actualStats.recovery;
                }
            }
        }
    }
    public float CurrentMoveSpeed
    {
        get { return MoveSpeed; }
        set { MoveSpeed = value; }
    }
    public float MoveSpeed
    {
        get { return actualStats.moveSpeed; }
        set
        {
            // Check if the value has changed
            if(actualStats.moveSpeed != value)
            {
                actualStats.moveSpeed = value;
                // Update the real time value of the stat
                // Add any additional logic here that needs to be executed when the value changes
                if(GameManager.instance != null)
                {
                        GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + actualStats.moveSpeed;
                }
            }
        }
    }
    public float CurrentMight
    {
        get { return Might; }
        set { Might = value; }
    }
    public float Might
    {
        get { return actualStats.might; }
        set
        {
            // Check if the value has changed
            if(actualStats.might != value)
            {
                actualStats.might = value;
                // Update the real time value of the stat
                // Add any additional logic here that needs to be executed when the value changes
                if(GameManager.instance != null)
                {
                        GameManager.instance.currentMightDisplay.text = "Might: " + actualStats.might;
                }
            }
        }
    }
    public float CurrentProjectileSpeed
    {
        get { return Speed; }
        set { Speed = value; }
    }
    public float Speed
    {
        get { return actualStats.speed; }
        set
        {
            // Check if the value has changed
            if(actualStats.speed != value)
            {
                actualStats.speed = value;
                // Update the real time value of the stat
                // Add any additional logic here that needs to be executed when the value changes
                if(GameManager.instance != null)
                {
                        GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + actualStats.speed;
                }
            }
        }
    }
    public float CurrentMagnet
    {
        get { return Magnet; }
        set { Magnet = value; }
    }
    public float Magnet
    {
        get { return actualStats.magnet; }
        set
        {
            // Check if the value has changed
            if(actualStats.magnet != value)
            {
                actualStats.magnet = value;
                // Update the real time value of the stat
                // Add any additional logic here that needs to be executed when the value changes
                if(GameManager.instance != null)
                {
                        GameManager.instance.currentMagnetDisplay.text = "Magnet: " + actualStats.magnet;
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

    PlayerInventory inventory;
    public int weaponIndex;
    public int passivItemIndex;

    public List<LevelRange> levelRange;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TextMeshProUGUI levelText;

    [Header("Visuals")]
    public ParticleSystem damageEffect;

    

    void Awake()
    {
       characterData = CharacterSelector.GetData();
       CharacterSelector.instance.DestroySingleton();

       inventory = GetComponent<PlayerInventory>();

       // Assign the variables
        baseStats = actualStats = characterData.stats;
        health = actualStats.maxHealth;
    }

    void Start()
    {
        //Spawn the starting Weapon
        inventory.Add(characterData.StartingWeapon);

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

    public void RecalculateStats()
    {
        actualStats = baseStats;
        foreach (PlayerInventory.Slot s in inventory.passiveSlots)
        {
            Passive p = s.item as Passive;
            if(p)
            {
                actualStats += p.GetBoosts();
            }
        }
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
        healthBar.fillAmount = health /actualStats.maxHealth;
    }

    public void Kill()
    {
       Debug.Log("Player is Dead");
       if(!GameManager.instance.isGameOver)
       {
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.GameOver();
       }
    }

    public void RestoreHealth(float amount)
    {
        // Only healthe player if their health is below the max health
        if(CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;

            // Makes sure the player does not go over the max health
            if(CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
            UpdateHealthBar();
        }
        
    }

    void Recover()
    {
        // Only recover the player if their health is below the max health
        if(CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;

            // Makes sure the player does not go over the max health
            if(CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
            UpdateHealthBar();
        }
    }
}
