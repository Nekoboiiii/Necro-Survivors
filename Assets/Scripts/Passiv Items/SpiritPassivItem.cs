using UnityEngine;

public class SpiritPassivItem : PassivItem
{
    protected override void ApplyModifier()
    {
        player.CurrentMight *= 1 + passivItemData.Multipler / 100f;
    }
}