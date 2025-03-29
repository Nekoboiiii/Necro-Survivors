using System.Collections.Generic;
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

    public void AddWeapon(int slotIndex, WeaponController weapon) // Add a weapon to a specific slot
    {
       weaponSlots[slotIndex] = weapon;
       weaponLevels[slotIndex] = weapon.weaponData.Level;
       weaponUISlots[slotIndex].enabled = true;
       weaponUISlots[slotIndex].sprite = weapon.weaponData.Icon; // Update the UI with the weapon icon
    }

    public void AddPassivItem(int slotIndex, PassivItem passivItem) // Add a passive item to a specific slot
    {
        passivItemSlots[slotIndex] = passivItem;
        passivItemLevels[slotIndex] = passivItem.passivItemData.Level;
        passivItemUISlots[slotIndex].enabled = true;
        passivItemUISlots[slotIndex].sprite = passivItem.passivItemData.Icon; // Update the UI with the passive item icon
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
        }
    }
}