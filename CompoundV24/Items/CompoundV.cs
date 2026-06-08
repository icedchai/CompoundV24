using System.Collections.Generic;
using System.Linq;
using CompoundV24.API.Features;
using CompoundV24.API.Features.Powers;
using CustomItemsAPI.Helpers;
using CustomItemsAPI.Items;
using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MEC;
using UnityEngine;

namespace CompoundV24.Items;

/// <summary>
/// The V24/TempV CustomItem.
/// </summary>
public class CompoundV : CustomUsableBase
{
    /// <inheritdoc/>
    public override string CustomItemName { get; } = "TemporaryV";

    /// <inheritdoc/>
    public override ItemType Type { get; } = ItemType.SCP1853;

    /// <inheritdoc/>
    public override string Description { get; } = "Use to gain a random superpower. Use more than one at your own risk.";

    /// <inheritdoc/>
    public override void OnUsing(Player player, UsableItem usableItem, TypeWrapper<bool> isAllowed)
    {
        isAllowed.Value = false;
        Pickup pu = usableItem.DropItem();
        pu.Destroy();

        int lifeId = player.RoleBase.UniqueLifeIdentifier;
        float cardiacArrestLength = 4.9f;
        List<Superpower> availablePowers = PowerManager.Instance.CompoundVPowers.Where(p => !p.Check(player)).ToList();
        if (availablePowers.Count == 1)
        {
            if (Random.Range(0, 1) < 0.5f)
            {
                cardiacArrestLength = 10f;
            }
        }

        player.DisableEffect<Scp1853>();

        player.EnableEffect<CardiacArrest>(1, 4.9f);
        Timing.CallDelayed(cardiacArrestLength + 0.05f, () =>
        {
            if (player is null || !player.IsAlive || player.RoleBase.UniqueLifeIdentifier != lifeId)
            {
                return;
            }

            if (availablePowers.IsEmpty())
            {
                return;
            }

            player.Heal(player.MaxHealth);

            Superpower powerToGive = availablePowers.RandomItem();
            powerToGive.Grant(player);
            player.SendHint(string.Format("You have gotten {0}\n{1}", powerToGive.Name, powerToGive.Description));

            SoundHelper.PlaySound("hallway");
        });
    }
}
