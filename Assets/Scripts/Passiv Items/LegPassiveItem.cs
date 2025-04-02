using NUnit.Framework.Internal.Commands;
using UnityEngine;

[System.Obsolete("Will be replaced by the new System")]
public class LegPassiveItem : PassivItem
{
    protected override void ApplyModifier()
    {
        player.CurrentMoveSpeed *= 1 + passivItemData.Multipler / 100f;
    }
}
