using NUnit.Framework.Internal.Commands;
using UnityEngine;

public class LegPassiveItem : PassivItem
{
    protected override void ApplyModifier()
    {
        player.CurrentMoveSpeed *= 1 + passivItemData.Multipler / 100f;
    }
}
