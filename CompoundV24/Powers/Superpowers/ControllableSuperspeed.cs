namespace CompoundV24.Powers.Superpowers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using MEC;
    using UnityEngine;
    using UserSettings.ServerSpecific;

    /// <summary>
    /// Controllable superspeed.
    /// </summary>
    public class ControllableSuperspeed : Superpower
    {
        /// <inheritdoc/>
        public override string Name => "superspeed_controlled";

        /// <summary>
        /// Gets or sets the lookup table between <see cref="Player"/>'s and a value indicating whether they have the power enabled.
        /// </summary>
        protected Dictionary<Player, bool> PlayerEnabledSpeed { get; set; } = new Dictionary<Player, bool>();

        /// <summary>
        /// Gets or sets the lookup table between <see cref="Player"/>'s and the intensity of their <see cref="EffectType.MovementBoost"/> without the power enabled.
        /// </summary>
        protected Dictionary<Player, byte> SavedSpeeds { get; set; } = new Dictionary<Player, byte>();

        /// <summary>
        /// Gets or sets the movement speed intensity to add to a <see cref="Player"/> with the power enabled.
        /// </summary>
        public byte MovementSpeedIntensity { get; set; } = 255;

        /// <summary>
        /// Checks whether the player has the power enabled.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>A value indicating whether <paramref name="player"/> has the power enabled.</returns>
        public bool PlayerHasSpeedEnabled(Player player)
        {
            return PlayerEnabledSpeed.ContainsKey(player) && PlayerEnabledSpeed[player];
        }

        /// <inheritdoc/>
        public override void OnUsedAbility(Player player)
        {
            base.OnUsedAbility(player);
            ToggleSpeed(player);
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Server.RestartingRound += ResetVars;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            Exiled.Events.Handlers.Server.RestartingRound -= ResetVars;
        }

        protected void ResetVars()
        {
            PlayerEnabledSpeed = new ();
            SavedSpeeds = new ();
        }

        /// <inheritdoc/>
        protected override void OnChangingRole(ChangingRoleEventArgs e)
        {
            base.OnChangingRole(e);
            PlayerEnabledSpeed.Remove(e.Player);
            SavedSpeeds.Remove(e.Player);
        }

        /// <inheritdoc/>
        protected override void ApplyProperties(Player player)
        {
            base.ApplyProperties(player);
            GameObject objct = GameObject.CreatePrimitive(PrimitiveType.Cube);
            objct.transform.parent = player.Transform;
            objct.transform.localPosition = Vector3.zero;
            RobinKillComponent robinkill = objct.AddComponent<RobinKillComponent>();
            robinkill.SuperspeedInstance = this;
            robinkill.Player = player;
            robinkill.Init();
        }

        /// <inheritdoc/>
        protected override void RemoveProperties(Player player)
        {
            base.RemoveProperties(player);
            RobinKillComponent robinkill = player.GameObject.GetComponentInChildren<RobinKillComponent>();
            GameObject.Destroy(robinkill.gameObject);
        }

        /// <summary>
        /// Toggles the speed for <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The player for whom to toggle the speed for.</param>
        protected void ToggleSpeed(Player player)
        {
            if (!PlayerEnabledSpeed.TryGetValue(player, out var status))
            {
                SavedSpeeds.Add(player, player.GetEffect(EffectType.MovementBoost)?.Intensity ?? 0);
                PlayerEnabledSpeed.Add(player, false);
            }

            if (status)
            {
                DisableSpeed(player);
            }
            else
            {
                EnableSpeed(player);
            }
        }

        /// <summary>
        /// Enables the speed on <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The player to enable the power on.</param>
        protected void EnableSpeed(Player player)
        {
            PlayerEnabledSpeed[player] = true;
            SavedSpeeds[player] = player.GetEffect(EffectType.MovementBoost)?.Intensity ?? 0;
            player.EnableEffect(EffectType.Invigorated);
            player.GetEffect(EffectType.MovementBoost).Intensity = MovementSpeedIntensity;
        }

        /// <summary>
        /// Disables the speed on <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The player to disable the power on.</param>
        protected void DisableSpeed(Player player)
        {
            player.GetEffect(EffectType.MovementBoost).Intensity = SavedSpeeds[player];
            player.DisableEffect(EffectType.Invigorated);
            PlayerEnabledSpeed[player] = false;
        }
    }
}
