namespace CompoundV24.Items
{
    using CompoundV24.Powers;
    using Exiled.API.Enums;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using MEC;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The V24/TempV CustomItem.
    /// </summary>
    [CustomItem(ItemType.SCP1853)]
    public class CompoundV : CustomItem
    {
        /// <inheritdoc/>
        public override uint Id { get; set; } = 1205;

        /// <inheritdoc/>
        public override string Name { get; set; } = "Compound V24";

        /// <inheritdoc/>
        public override string Description { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override ItemType Type { get; set; } = ItemType.SCP1853;

        /// <inheritdoc/>
        public override float Weight { get; set; } = 0.5f;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; }

        /// <inheritdoc/>
        public override bool ShouldMessageOnGban => true;

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Player.UsedItem += OnUsed;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            Exiled.Events.Handlers.Player.UsedItem -= OnUsed;
        }

        private void OnUsed(UsedItemEventArgs e)
        {
            if (!Check(e.Item))
            {
                return;
            }

            e.Player.GetEffect(EffectType.Scp1853).Intensity = 0;

            e.Player.EnableEffect(EffectType.CardiacArrest, 5f);
            Timing.CallDelayed(5f, () =>
            {
                if (e.Player is null || e.Player.IsDead)
                {
                    return;
                }

                e.Player.Heal(e.Player.MaxHealth);

                List<Superpower> availablePowers = PowerManager.Instance.CompoundVPowers.Where(p => !p.Check(e.Player)).ToList();
                if (availablePowers.IsEmpty())
                {
                    return;
                }

                Superpower powerToGive = availablePowers.RandomItem();
                powerToGive.Grant(e.Player);
                e.Player.ShowHint(string.Format("You have gotten {0}\n{1}", powerToGive.Name, powerToGive.Description));

                if (PowerManager.Instance.PlayersToPowers[e.Player].Count > 1)
                {
                    SoundHelper.PlaySound("hallway");
                }
            });
        }
    }
}
