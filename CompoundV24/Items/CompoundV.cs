namespace CompoundV24.Items
{
    using System.Collections.Generic;
    using System.Linq;
    using ColdWaterLibrary.Audio.Features.Helpers;
    using CompoundV24.API.Features.Powers;
    using CustomItemsAPI.Items;
    using CustomPlayerEffects;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Features.Wrappers;
    using MEC;

    /// <summary>
    /// The V24/TempV CustomItem.
    /// </summary>
    public class CompoundV : CustomUsableBase
    {
        /// <inheritdoc/>
        public override string CustomItemName { get; } = "CompoundV";

        /// <inheritdoc/>
        public override ItemType Type { get; } = ItemType.SCP1853;

        /// <inheritdoc/>
        public override string Description { get; } = "Use to gain a random superpower.";

        /// <inheritdoc/>
        public override void OnUsed(Player player, UsableItem usableItem)
        {
            player.DisableEffect<Scp1853>();

            player.EnableEffect<CardiacArrest>(1, 5);
            Timing.CallDelayed(5f, () =>
            {
                if (player is null || !player.IsAlive)
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
}
