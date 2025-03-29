using UnityEngine;

public class ExperienceOrb : Pickup, ICollectible
{
   public int experienceGranted;
   
    public void Collect()
    {
        PlayerStats player = FindFirstObjectByType<PlayerStats>();
        player.IncreaseExperience(experienceGranted);
    }
}
