using UnityEngine;

public class HeallthPotion : Pickup, ICollectible
{
    public int healthToRestore;
    public void Collect()
    {
        PlayerStats player = FindFirstObjectByType<PlayerStats>();
        player.RestoreHealth(healthToRestore);
    }
}
