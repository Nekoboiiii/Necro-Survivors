using UnityEngine;

/// <summary>
/// Replcement for the PassivItemScriptableObject class. The idea is we want to store all
/// passiv item level data in one single object, instead of having multiple objects to store a single passiv item
/// </summary>

public class PassiveData : ItemData
{
    public Passive.Modifier baseStats;
    public Passive.Modifier[] growth;

    public Passive.Modifier GetLevelData(int level)
    {
        // Pick the stats from the next level
        if(level - 2 < growth.Length)
        {
            return growth[level - 2];
        }

        // Return an empy value and a warning
        Debug.LogWarning(string.Format("Passive doesnt have its level up stats confiqured for level {0}!", level));
        return new Passive.Modifier();
    }
}