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
    public override string Description { get; } = "Use to gain a random superpower.";

    /// <inheritdoc/>
    public override void OnUsing(Player player, UsableItem usableItem, TypeWrapper<bool> isAllowed)
    {
        isAllowed.Value = false;
        Pickup pu = usableItem.DropItem();
        pu.Destroy();

        int lifeId = player.RoleBase.UniqueLifeIdentifier;

        player.DisableEffect<Scp1853>();

        player.EnableEffect<CardiacArrest>(1, 4.9f);
        Timing.CallDelayed(5f, () =>
        {
            if (player is null || !player.IsAlive || player.RoleBase.UniqueLifeIdentifier != lifeId)
            {
                return;
            }

            player.Heal(player.MaxHealth);

            List<Superpower> availablePowers = PowerManager.Instance.CompoundVPowers.Where(p => !p.Check(player)).ToList();
            if (availablePowers.IsEmpty())
            {
                return;
            }

            Superpower powerToGive = availablePowers.RandomItem();
            powerToGive.Grant(player);
            player.SendHint(string.Format("You have gotten {0}\n{1}", powerToGive.Name, powerToGive.Description));

            if (PowerManager.Instance.PlayersToPowers[player].Count > 1)
            {
                SoundHelper.PlaySound("hallway");
            }
        });
    }
}
