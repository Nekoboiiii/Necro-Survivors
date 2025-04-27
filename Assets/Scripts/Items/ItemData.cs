using UnityEngine;

/// <summary>
/// Base class for all weapons / passiv items. The base calss is used so that both WeaponData
/// and PassivItemData are able to be used interchangeably if required
/// </summary>

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int maxLevel = 5;
    public Evolution[] evolutionData;

    [System.Serializable]
    public class Evolution
    {
        public enum Condition { auto, manual, treasureChest }
        [System.Flags]
        public enum Consumption { none = 0, weapons = 1, passives = 2, all = 3 }

        public Condition condition;
        public Consumption consumes;
        public int evolutionLevel = 1;
        public Config[] catalysts;
        public Outcome outcome;

        [System.Serializable]
        public struct Config
        {
            public ItemData itemType;
            public int level;
        }

        [System.Serializable]
        public struct Outcome
        {
            public ItemData itemType;
        }
    }
}