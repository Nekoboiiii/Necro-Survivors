using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Obsolete("Will be replaced by the new System")]
public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    public int[] weaponLevels = new int[6];
    public List<Image> weaponUISlots = new List<Image>(6);
    public List<PassivItem> passivItemSlots = new List<PassivItem>(6);
    public int[] passivItemLevels = new int[6];
    public List<Image> passivItemUISlots = new List<Image>(6);

    #region 
    [System.Serializable]
    public class WeaponUpgrade
    {
        public int weaponUpgradeIndex;
        public GameObject ininitalWeapon;
        public WeaponScriptableObject weaponData;
    }

    [System.Serializable]
    public class PassivItemUpgrade
    {
        public int passivItemUpgradeIndex;
        public GameObject ininitalPassivItem;
        public PassivItemScriptableObject passivItemData;
    }

    [System.Serializable]
    public class UpgradeUi
    {
        public TextMeshProUGUI upgradeNameDisplay;
        public TextMeshProUGUI upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }
    #endregion

    public List<WeaponUpgrade> weaponUpgradesOptions = new List<WeaponUpgrade>(); // List of weapon upgrades
    public List<PassivItemUpgrade> passivItemUpgradesOptions = new List<PassivItemUpgrade>(); // List of passive item upgrades
    public List<UpgradeUi> upgradeUiOptions = new List<UpgradeUi>(); // List of upgrade UI options
    public List<WeaponEvolutionBlueprint> weaponEvolutions = new List<WeaponEvolutionBlueprint>();

    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>(); // Get the PlayerStats component attached to the player
    }
    public void AddWeapon(int slotIndex, WeaponController weapon) // Add a weapon to a specific slot
    {
       weaponSlots[slotIndex] = weapon;
       weaponLevels[slotIndex] = weapon.weaponData.Level;
       weaponUISlots[slotIndex].enabled = true;
       weaponUISlots[slotIndex].sprite = weapon.weaponData.Icon; // Update the UI with the weapon icon

       if(GameManager.instance != null && GameManager.instance.ischoosingUpgrade)
       {
        GameManager.instance.EndLevelUp();
       }
    }

    public void AddPassivItem(int slotIndex, PassivItem passivItem) // Add a passive item to a specific slot
    {
        passivItemSlots[slotIndex] = passivItem;
        passivItemLevels[slotIndex] = passivItem.passivItemData.Level;
        passivItemUISlots[slotIndex].enabled = true;
        passivItemUISlots[slotIndex].sprite = passivItem.passivItemData.Icon; // Update the UI with the passive item icon
        
        if(GameManager.instance != null && GameManager.instance.ischoosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if(weaponSlots.Count > slotIndex)
        {
            
            WeaponController weapon = weaponSlots[slotIndex];
            if(!weapon.weaponData.NextLevelPrefab) // Check if the weapon has a next level prefab
            {
                Debug.LogError("This weapon cannot be upgraded further.");
                return;
            }
            GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, weapon.transform.position, Quaternion.identity);
            upgradedWeapon.transform.SetParent(transform);
            AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>()); // Add the upgraded weapon to the inventory
            Destroy(weapon.gameObject); // Destroy the old weapon
            weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().weaponData.Level; // Increment the level of the weapon
            
            weaponUpgradesOptions[upgradeIndex].weaponData = upgradedWeapon.GetComponent<WeaponController>().weaponData;
            
            if(GameManager.instance != null && GameManager.instance.ischoosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
        

    }

    public void LevelUpPassivItem(int slotIndex, int upgradeIndex)
    {
        if(passivItemSlots.Count > slotIndex)
        {
            PassivItem PassivItem = passivItemSlots[slotIndex];
            if(!PassivItem.passivItemData.NextLevelPrefab) // Check if the weapon has a next level prefab
            {
                Debug.LogError("This Passiv Item cannot be upgraded further.");
                return;
            }
            GameObject upgradedPassivItem = Instantiate(PassivItem.passivItemData.NextLevelPrefab, PassivItem.transform.position, Quaternion.identity);
            upgradedPassivItem.transform.SetParent(transform);
            AddPassivItem(slotIndex, upgradedPassivItem.GetComponent<PassivItem>()); // Add the upgraded passiv item to the inventory
            Destroy(PassivItem.gameObject); // Destroy the old passiv item
            passivItemLevels[slotIndex] = upgradedPassivItem.GetComponent<PassivItem>().passivItemData.Level; // Increment the level of the weapon

            passivItemUpgradesOptions[upgradeIndex].passivItemData = upgradedPassivItem.GetComponent<PassivItem>().passivItemData;

            if(GameManager.instance != null && GameManager.instance.ischoosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    public void ApplyUpgradeOptions()
    {
        List<WeaponUpgrade> availableWeaponUpgrades = new(weaponUpgradesOptions);
        List<PassivItemUpgrade> availablePassivItemsUpgrades = new(passivItemUpgradesOptions);

        foreach (var upgradeOption in upgradeUiOptions)
        {
           
            if(availableWeaponUpgrades.Count == 0 && availablePassivItemsUpgrades.Count == 0)    
            {
                return;
            }

            int upgradeType;

            if(availableWeaponUpgrades.Count == 0)
            {
                upgradeType = 2;
            }
            else if (availablePassivItemsUpgrades.Count == 0)
            {
                upgradeType = 1;
            }
            else
            {
                upgradeType = Random.Range(1, 3); //Choose between weapon and passive items
            }

            if (upgradeType == 1) // Weapon upgrade
            {
                if (availableWeaponUpgrades.Count == 0)
                {
                    Debug.LogWarning("No available weapon upgrades.");
                    continue; // Skip if no weapon upgrades are available
                }

                WeaponUpgrade chosenWeaponUpgrade = availableWeaponUpgrades[Random.Range(0, availableWeaponUpgrades.Count)];
                availableWeaponUpgrades.Remove(chosenWeaponUpgrade);

                if (chosenWeaponUpgrade != null)
                {

                    EnableUpgradeUI(upgradeOption);

                    bool newWeapon = false;
                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        if (weaponSlots[i] != null && weaponSlots[i].weaponData == chosenWeaponUpgrade.weaponData)
                        {
                            newWeapon = false;
                            if (!newWeapon)
                            {
                                if (!chosenWeaponUpgrade.weaponData.NextLevelPrefab)
                                {
                                    DisableUpgradeUI(upgradeOption);
                                    Debug.LogWarning("No next level prefab for weapon.");
                                    break;
                                }

                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, chosenWeaponUpgrade.weaponUpgradeIndex));
                                var nextLevelWeaponData = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData;
                                upgradeOption.upgradeDescriptionDisplay.text = nextLevelWeaponData.Description;
                                upgradeOption.upgradeNameDisplay.text = nextLevelWeaponData.Name;
                            }
                            break;
                        }
                        else
                        {
                            newWeapon = true;
                        }
                    }
                    if (newWeapon)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnWeapon(chosenWeaponUpgrade.ininitalWeapon));
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.Description;
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.Name;
                    }

                    upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.weaponData.Icon;
                }
            }
            else if (upgradeType == 2) // Passive item upgrade
            {
                if (availablePassivItemsUpgrades.Count == 0)
                {
                    Debug.LogWarning("No available passive item upgrades.");
                    continue; // Skip if no passive item upgrades are available
                }

                PassivItemUpgrade chosenPassivItemUpgrade = availablePassivItemsUpgrades[Random.Range(0, availablePassivItemsUpgrades.Count)];
                availablePassivItemsUpgrades.Remove(chosenPassivItemUpgrade);

                if (chosenPassivItemUpgrade != null)
                {
                    
                    EnableUpgradeUI(upgradeOption);
                    
                    bool newPassivItem = false;
                    for (int i = 0; i < passivItemSlots.Count; i++)
                    {
                        if (passivItemSlots[i] != null && passivItemSlots[i].passivItemData == chosenPassivItemUpgrade.passivItemData)
                        {
                            newPassivItem = false;
                            if (!newPassivItem)
                            {
                                if (!chosenPassivItemUpgrade.passivItemData.NextLevelPrefab)
                                {
                                    DisableUpgradeUI(upgradeOption);
                                    Debug.LogWarning("No next level prefab for passive item.");
                                    break;
                                }

                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassivItem(i, chosenPassivItemUpgrade.passivItemUpgradeIndex));
                                var nextLevelPassivItemData = chosenPassivItemUpgrade.passivItemData.NextLevelPrefab.GetComponent<PassivItem>().passivItemData;
                                upgradeOption.upgradeDescriptionDisplay.text = nextLevelPassivItemData.Description;
                                upgradeOption.upgradeNameDisplay.text = nextLevelPassivItemData.Name;
                            }
                            break;
                        }
                        else
                        {
                            newPassivItem = true;
                        }
                    }
                    if (newPassivItem)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnPassivItem(chosenPassivItemUpgrade.ininitalPassivItem));
                        upgradeOption.upgradeDescriptionDisplay.text = chosenPassivItemUpgrade.passivItemData.Description;
                        upgradeOption.upgradeNameDisplay.text = chosenPassivItemUpgrade.passivItemData.Name;
                    }

                    upgradeOption.upgradeIcon.sprite = chosenPassivItemUpgrade.passivItemData.Icon;
                }
            }
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach (var upgradeOption in upgradeUiOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    void DisableUpgradeUI(UpgradeUi ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }
    
    void EnableUpgradeUI(UpgradeUi ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }

    public List<WeaponEvolutionBlueprint> GetPossibleEvolutions()
    {
        List<WeaponEvolutionBlueprint> possibleEvolutions = new List<WeaponEvolutionBlueprint>();
        foreach (WeaponController weapon in weaponSlots)
        {
            if( weapon != null)
            {
                foreach (PassivItem catalyst in passivItemSlots)
                {
                    if(catalyst != null)
                    {
                        foreach (WeaponEvolutionBlueprint evolution in weaponEvolutions)
                        {
                            if(weapon.weaponData.Level >= evolution.baseWeaponData.Level && catalyst.passivItemData.Level >= evolution.catalystPassiveItenData.Level)
                            {
                                possibleEvolutions.Add(evolution);
                            }
                        }
                    }
                }
            }
        }
        return possibleEvolutions;
    }
    
    public void EvolveWeapon(WeaponEvolutionBlueprint evolution)
    {
        for (int weaponSlotIndex = 0; weaponSlotIndex < weaponSlots.Count; weaponSlotIndex++)
        {
            WeaponController weapon = weaponSlots[weaponSlotIndex];

            if (!weapon)
            {
                continue;
            }

            for (int catalystSlotIndex = 0; catalystSlotIndex < passivItemSlots.Count; catalystSlotIndex++)
            {
                PassivItem catalyst = passivItemSlots[catalystSlotIndex];
                if (!catalyst)
                {
                    continue;
                }

                if (weapon && catalyst && weapon.weaponData.Level >= evolution.baseWeaponData.Level && catalyst.passivItemData.Level >= evolution.catalystPassiveItenData.Level)
                {
                    GameObject evolvedWeapon = Instantiate(evolution.evolvedWeapon, transform.position, Quaternion.identity);
                    WeaponController evolvedWeaponController = evolvedWeapon.GetComponent<WeaponController>();

                    evolvedWeapon.transform.SetParent(transform); // Set the weapon to be a child of the player
                    AddWeapon(weaponSlotIndex, evolvedWeaponController);
                    Destroy(weapon.gameObject);

                    // Update level and icon
                    weaponLevels[weaponSlotIndex] = evolvedWeaponController.weaponData.Level;
                    weaponUISlots[weaponSlotIndex].sprite = evolvedWeaponController.weaponData.Icon;

                    //Update the upgrade options
                    weaponUpgradesOptions.RemoveAt(evolvedWeaponController.weaponData.EvolvedUpgradeToRemove);

                    Debug.LogWarning("Evolved");
                    return;
                }            
            }
        }
    }
}