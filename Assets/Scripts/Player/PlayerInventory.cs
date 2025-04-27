using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public Item item;
        public Image image;

        public void Assign(Item assignedItem)
        {
            item = assignedItem;
            if (item is Weapon)
            {
                Weapon w = item as Weapon;
                image.enabled = true;
                image.sprite = w.data.icon;
            }
            else
            {
                Passive p = item as Passive;
                image.enabled = true;
                image.sprite = p.data.icon;
            }
            Debug.Log(string.Format("Assigned {0} to player", item.name));
        }

        public void Clear()
        {
            item = null;
            image.enabled = false;
            image.sprite = null;
        }

        public bool IsEmpty()
        {
            return item == null;
        }
    }

    public List<Slot> weaponSlots = new List<Slot>(); // List of weapon slots in the inventory
    public List<Slot> passiveSlots = new List<Slot>(); // List of passive slots in the inventory

    [System.Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    [Header("UI Elements")]
    public List<WeaponData> availableWeapons = new List<WeaponData>(); // List of upgrade options for weapons
    public List<PassiveData> availablePassives = new List<PassiveData>(); // List of upgrade options for passive items
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>(); // List of ui for upgrade options present in the scene

    PlayerStats player;
    
    private void Start()
    {
        player = GetComponent<PlayerStats>();
    }

    // Checks if the inventory has an item of a certain type
    public bool Has(ItemData type)
    {
        return Get(type) != null;
    }

    public Item Get(ItemData type)
    {
        if (type is WeaponData) 
        {
            return Get(type as WeaponData);
        }
        else if (type is PassiveData)
        {
            return Get(type as PassiveData);
        }
        return null;
    }

    // Find a passive of a certain type in the inventory
    public Passive Get(PassiveData type)
    {
        foreach (Slot s in passiveSlots)
        {
            if (s.item == null) continue;
            
            Passive p = s.item as Passive;
            if (p && p.data == type)
            {
                return p;
            }
        }
        return null;
    }
    
    // Find a weapon of a certain type in the inventory
    public Weapon Get(WeaponData type)
    {
        foreach (Slot s in weaponSlots)
        {
            if (s.item == null) continue;
            
            Weapon w = s.item as Weapon;
            if (w && w.data == type)
            {
                return w;
            }
        }
        return null;
    }

    // Removes a Weapon of a particular type as specified by <data>
    public bool Remove(WeaponData data, bool removeUpgradeAvailability = false)
    {
        // Remove this weapon from the upgrade pool
        if(removeUpgradeAvailability)
        {
            availableWeapons.Remove(data);
        }

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            if (weaponSlots[i].item == null) continue;
            
            Weapon w = weaponSlots[i].item as Weapon;
            if (w && w.data == data)
            {
                weaponSlots[i].Clear();
                w.OnUnequip();
                Destroy(w.gameObject);
                return true;
            }
        }
        return false;
    }

    // Removes a passive of a particular type as specified by <data>
    public bool Remove(PassiveData data, bool removeUpgradeAvailability = false)
    {
        // Remove this passive from the upgrade pool
        if(removeUpgradeAvailability)
        {
            availablePassives.Remove(data);
        }
        
        for (int i = 0; i < passiveSlots.Count; i++)
        {
            if (passiveSlots[i].item == null) continue;
            
            Passive p = passiveSlots[i].item as Passive;
            if (p && p.data == data)
            {
                passiveSlots[i].Clear();
                p.OnUnequip();
                Destroy(p.gameObject);
                return true;
            }
        }
        return false;
    }

    // If an ItemData is passed determine what type it is and call the respective overload
    // We also have an optional boolean to remove this item from the upgrade list
    public bool Remove(ItemData data, bool removeUpgradeAvailability = false)
    {
        if (data is PassiveData)
        {
            return Remove(data as PassiveData, removeUpgradeAvailability);
        }
        else if (data is WeaponData)
        {
            return Remove(data as WeaponData, removeUpgradeAvailability);
        }
        return false;
    }

    // Finds an empty slot and adds a weapon of a certain type 
    // returns the slot number that the item was put in
    public int Add(WeaponData data)
    {
        int slotNum = -1;

        // Try to find an empty Slot
        for (int i = 0; i < weaponSlots.Count; i++)
        {
            if (weaponSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }
        // If there is no empty slot exit
        if (slotNum < 0)
        {
            return slotNum;
        } 

        // Otherwise create the weapon in the slot
        // Get the type of the weapon we want to spawn
        Type weaponType = Type.GetType(data.behavior);
        // If that returns null, try with the namespace
        if (weaponType == null)
        {
            // Try common namespaces - adjust these based on your project structure
            weaponType = Type.GetType("Weapons." + data.behavior) ?? 
                         Type.GetType(GetType().Namespace + "." + data.behavior) ??
                         FindWeaponType(data.behavior);
        }

        if(weaponType != null)
        {
            // Spawn the weapon GameObject
            GameObject go = new GameObject(data.baseStats.name + " Controller");
            Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);
            spawnedWeapon.Initialise(data);
            spawnedWeapon.transform.SetParent(transform); // Set the weapon to be a child of the player
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.OnEquip();

            // Assign the weapon to the slot
            weaponSlots[slotNum].Assign(spawnedWeapon);

            // Close the level up UI if it is on
            if (GameManager.instance != null)
            {
                if (GameManager.instance.ischoosingUpgrade)
                {
                    GameManager.instance.EndLevelUp();
                }
            }
            return slotNum;
        }
        else
        {
            Debug.LogWarning(string.Format("Invalid weapon type specified for {0}.", data.name));
        }

        return -1;
    }

    // Finds an empty slot and adds a passive of a certain type 
    // returns the slot number that the item was put in
    public int Add(PassiveData data)
    {
        int slotNum = -1;

        // Try to find an empty Slot
        for (int i = 0; i < passiveSlots.Count; i++)
        {
            if (passiveSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }
        // If there is no empty slot exit
        if (slotNum < 0)
        {
            return slotNum;
        } 

        // Otherwise create the passive in the slot
        // Get the type of the passive we want to spawn
        GameObject go = new GameObject(data.baseStats.name + " Passive");
        Passive p = go.AddComponent<Passive>();
        p.Initialise(data);
        p.transform.SetParent(transform); // Set the passive to be a child of the player
        p.transform.localPosition = Vector2.zero;

        // Assign the passive to the slot
        passiveSlots[slotNum].Assign(p);

        // Close the level up UI if it is on
        if (GameManager.instance != null && GameManager.instance.ischoosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();

        return slotNum;
    }

    // If we don't know what item is being added, this function will determine that
    public int Add(ItemData data)
    {
        if (data is WeaponData)
        {
            return Add(data as WeaponData);
        }
        else if (data is PassiveData)
        {
            return Add(data as PassiveData);
        }
        return -1;
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (weaponSlots.Count > slotIndex)
        {
            Weapon weapon = weaponSlots[slotIndex].item as Weapon;

            // Don't level up the weapon if it is already at max level
            if (!weapon.DoLevelUp())
            {
                Debug.LogWarning(string.Format("Failed to level up {0}", weapon.name));
                return;
            }
        }

        if (GameManager.instance != null && GameManager.instance.ischoosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if (passiveSlots.Count > slotIndex)
        {
            Passive p = passiveSlots[slotIndex].item as Passive;

            // Don't level up the passive item if it is already at max level
            if (!p.DoLevelUp())
            {
                Debug.LogWarning(string.Format("Failed to level up {0}", p.name));
                return;
            }
        }

        if (GameManager.instance != null && GameManager.instance.ischoosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();
    }

    // Determines what upgrade should appear
    void ApplyUpgradeOptions()
    {
        // Make a duplicate of the available weapon / passive upgrade lists
        // so we can iterate through them in the function
        List<WeaponData> availableWeaponUpgrades = new List<WeaponData>(availableWeapons);
        List<PassiveData> availablePassiveItemUpgrades = new List<PassiveData>(availablePassives);

        // Iterate through each slot in the upgrade UI
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            // If there are no more available upgrades then we abort
            if (availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0)
            {
                return;
            }        

            // Determine whether this upgrade should be for passive or active weapons
            int upgradeType;
            if (availableWeaponUpgrades.Count == 0)
            {
                upgradeType = 2;
            }
            else if (availablePassiveItemUpgrades.Count == 0)
            {
                upgradeType = 1;
            }
            else
            {
                // Random generates a number between 1 and 2
                upgradeType = UnityEngine.Random.Range(1, 3);
            }

            // Generates an active weapon upgrade
            if (upgradeType == 1)
            {
                // Pick a weapon upgrade, then remove it so that we don't get it twice
                WeaponData chosenWeaponUpgrade = availableWeaponUpgrades[UnityEngine.Random.Range(0, availableWeaponUpgrades.Count)];
                availableWeapons.Remove(chosenWeaponUpgrade);

                // Ensure that the selected weapon data is valid
                if (chosenWeaponUpgrade != null)
                {
                    // Turns on the UI slot
                    EnableUpgradeUI(upgradeOption);
                    
                    // Loops through all existing weapons. If it finds a match, it will
                    // hook an event listener to the button that will level up the weapon
                    // when this upgrade option is clicked
                    bool isLevelUp = false;
                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        Weapon w = weaponSlots[i].item as Weapon;
                        if (w != null && w.data == chosenWeaponUpgrade)
                        {
                            // If the weapon is already at the max level do not allow upgrade
                            if (chosenWeaponUpgrade.maxLevel <= w.currentLevel)
                            {
                                // DisableUpgradeUI(upgradeOption)
                                isLevelUp = false;
                                break;
                            }

                            // Set the Event Listener item and level description to be that of the next level
                            int capturedIndex = i; // Capture the loop variable
                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(capturedIndex, capturedIndex)); // Apply button functionality
                            Weapon.Stats nextLevel = chosenWeaponUpgrade.GetLevelData(w.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;   
                            isLevelUp = true;
                            break;
                        }
                    }

                    // If the code gets here it means that it will be adding a new weapon instead of upgrading an existing weapon
                    if (!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenWeaponUpgrade)); // Apply button functionality
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.baseStats.description; // Apply initial description
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.baseStats.name; // Apply initial name
                        upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                    }                       
                }
            }
            else if (upgradeType == 2)
            {

                // Note Need to recode this system as right now it disables an upgrade slot if 
                // it hits a weapon that already reached max level
                PassiveData chosenPassivUpgrade = availablePassiveItemUpgrades[UnityEngine.Random.Range(0, availablePassiveItemUpgrades.Count)];
                availablePassiveItemUpgrades.Remove(chosenPassivUpgrade);

                if (chosenPassivUpgrade != null)
                {
                    // Turns on the UI slot
                    EnableUpgradeUI(upgradeOption);
                    
                    // Loops through all existing passive. If it finds a match, it will
                    // hook an event listener to the button that will level up the weapon
                    // when this upgrade option is clicked
                    bool isLevelUp = false;
                    for (int i = 0; i < passiveSlots.Count; i++)
                    {
                        Passive p = passiveSlots[i].item as Passive;
                        if (p != null && p.data == chosenPassivUpgrade)
                        {
                            // If the passive is already at the max level do not allow upgrade
                            if (chosenPassivUpgrade.maxLevel <= p.currentLevel)
                            {
                                // DisableUpgradeUI(upgradeOption)
                                isLevelUp = false;
                                break;
                            }

                            // Set the Event Listener item and level description to be that of the next level
                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, i)); // Apply button functionality
                            Passive.Modifier nextLevel = chosenPassivUpgrade.GetLevelData(p.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeIcon.sprite = chosenPassivUpgrade.icon;   
                            isLevelUp = true;
                            break;
                        }
                    }

                    // Spawn a new passive item
                    if (!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenPassivUpgrade)); // Apply button functionality
                        Passive.Modifier nextLevel = chosenPassivUpgrade.baseStats;
                        upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description; // Apply initial description
                        upgradeOption.upgradeNameDisplay.text = nextLevel.name; // Apply initial name
                        upgradeOption.upgradeIcon.sprite = chosenPassivUpgrade.icon;
                    }                       
                }
            }
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption); // Call the DisableUpgradeUI method here to disable all UI options applying upgrades to them
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }

    private Type FindWeaponType(string typeName)
    {
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(Weapon)) && type.Name == typeName)
                {
                    return type;
                }
            }
        }
        return null;
    }
}