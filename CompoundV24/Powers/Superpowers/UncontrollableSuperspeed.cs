namespace CompoundV24.Powers.Superpowers
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using MEC;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using UnityEngine;
    using UserSettings.ServerSpecific;

    /// <summary>
    /// Controllable superspeed.
    /// </summary>
    public class UncontrollableSuperspeed : ControllableSuperspeed
    {
        /// <inheritdoc/>
        public override string Name => "superspeed_uncontrolled";

        /// <inheritdoc/>
        public override void OnUsedAbility(Player player)
        {
            base.OnUsedAbility(player);

            if (PlayerHasSpeedEnabled(player))
            {
                return;
            }
            else
            {
                ToggleSpeed(player);
                Timing.CallDelayed(10f, () =>
                {
                    if (!PlayerHasSpeedEnabled(player))
                    {
                        return;
                    }

                    ToggleSpeed(player);
                    if (UnityEngine.Random.Range(0, 100) < 30)
                    {
                        player.EnableEffect(EffectType.CardiacArrest, 4);
                    }
                });
            }
        }

        /// <inheritdoc/>
        protected override void OnChangingRole(ChangingRoleEventArgs e)
        {
            base.OnChangingRole(e);
            PlayerEnabledSpeed.Remove(e.Player);
            SavedSpeeds.Remove(e.Player);
        }
    }
}
