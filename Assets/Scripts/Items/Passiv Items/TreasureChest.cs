using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        PlayerInventory p = col.GetComponent<PlayerInventory>();
        if(p)
        {
            bool randomBool = Random.Range(0, 2) == 0;

            OpenTreasureChest(p, randomBool);
            Destroy(gameObject);
        }
    }

    public void OpenTreasureChest(PlayerInventory inventory, bool isHigherTier)
    {
        // Loop through every weapon to check whether it can evolve
        foreach (PlayerInventory.Slot s in inventory.weaponSlots)
        {
            Weapon w = s.item as Weapon;
            // Ignore weapon if it's null or cannot evolve
            if(w == null || w.data == null || w.data.evolutionData == null)
            {
                continue;
            }

            // Loop through every possible evolution of the weapon
            foreach (ItemData.Evolution e in w.data.evolutionData)
            {
                // Only attempt to evolve weapons via treasure chest evolution
                if (e.condition == ItemData.Evolution.Condition.treasureChest)
                {
                    bool attempt = w.AttemptEvolution(e, 0);
                    // If evolution suceeds stop
                    if(attempt)
                    {
                        return; 
                    }
                }
            }
        }
    }
}
