using UnityEngine;

[System.Obsolete("Will be replaced by the new System")]
public class SpiritPassivItem : PassivItem
{
    protected override void ApplyModifier()
    {
        player.CurrentMight *= 1 + passivItemData.Multipler / 100f;
    }
}