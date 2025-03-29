using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
        public GameObject ininitalWeapon;
        public WeaponScriptableObject weaponData;
    }

    [System.Serializable]
    public class PassivItemUpgrade
    {
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

    public void LevelUpWeapon(int slotIndex)
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
            
            if(GameManager.instance != null && GameManager.instance.ischoosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
        

    }

    public void LevelUpPassivItem(int slotIndex)
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

            if(GameManager.instance != null && GameManager.instance.ischoosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    public void ApplyUpgradeOptions()
    {
        foreach(var upgradeOption in upgradeUiOptions)
        {
            int upgradeType = Random.Range(1, 3); // Randomly select an upgrade type (0 for weapon, 1 for passive item)
            if(upgradeType == 1) // Weapon upgrade
            {
                WeaponUpgrade chosenWeaponUpgrade = weaponUpgradesOptions[Random.Range(0, weaponUpgradesOptions.Count)];


                if(chosenWeaponUpgrade != null)
                {
                    bool newWeapon = false;
                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        if(weaponSlots[i] != null && weaponSlots[i].weaponData == chosenWeaponUpgrade.weaponData)
                        {
                            newWeapon = false;
                            if(!newWeapon)
                            {
                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i)); // Assign the level up function to the button
                                // Set the description and name to be that of the next level
                                var nextLevelWeaponData = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData;
                                upgradeOption.upgradeDescriptionDisplay.text = nextLevelWeaponData.Description;
                                upgradeOption.upgradeNameDisplay.text = nextLevelWeaponData.Name; // Ensure the correct name is used
 }
                            break;
                        }
                        else
                        {
                            newWeapon = true;
                        }
                    }
                    if(newWeapon)
                    {
                       upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnWeapon(chosenWeaponUpgrade.ininitalWeapon)); // Assign the spawn weapon function to the button
                       // Apply initial description and name
                       upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.Description;
                       upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.Name;
                    }

                    upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.weaponData.Icon;
                }
                 
            }
            else if(upgradeType == 2) // Passive item upgrade
            {
                PassivItemUpgrade chosenPassivItemUpgrade = passivItemUpgradesOptions[Random.Range(0, passivItemUpgradesOptions.Count)];
                if(chosenPassivItemUpgrade != null)
                {
                    bool newPassivItem = false;
                    for (int i = 0; i < passivItemSlots.Count; i++)
                    {
                        if(passivItemSlots[i] != null && passivItemSlots[i].passivItemData == chosenPassivItemUpgrade.passivItemData)
                        {
                            newPassivItem = false;
                            if(!newPassivItem)
                            {
                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassivItem(i)); // Assign the level up function to the button
                                // Set the description and name to be that of the next level
                                var nextLevelPassivItemData = chosenPassivItemUpgrade.passivItemData.NextLevelPrefab.GetComponent<PassivItem>().passivItemData;
                                upgradeOption.upgradeDescriptionDisplay.text = nextLevelPassivItemData.Description;
                                upgradeOption.upgradeNameDisplay.text = nextLevelPassivItemData.Name; // Ensure the correct name is used
 }
                            break;
                        }
                        else
                        {
                            newPassivItem = true;
                        }
                    }
                    if(newPassivItem)
                    {
                       upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnPassivItem(chosenPassivItemUpgrade.ininitalPassivItem)); // Assign the spawn passiv item function to the button
                       // Apply initial description and name
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
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }
}