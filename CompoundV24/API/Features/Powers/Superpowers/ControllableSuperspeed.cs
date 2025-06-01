namespace CompoundV24.API.Features.Powers.Superpowers
{
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using UnityEngine;

    /// <summary>
    /// Controllable superspeed.
    /// </summary>
    public class ControllableSuperspeed : ToggleablePower
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "superspeed_controlled";

        /// <summary>
        /// Gets or sets the lookup table between <see cref="Player"/>'s and the intensity of their <see cref="EffectType.MovementBoost"/> without the power enabled.
        /// </summary>
        protected Dictionary<Player, byte> SavedSpeeds { get; set; } = new Dictionary<Player, byte>();

        /// <summary>
        /// Gets or sets the movement speed intensity to add to a <see cref="Player"/> with the power enabled.
        /// </summary>
        public byte MovementSpeedIntensity { get; set; } = 255;

        protected override void DisposeVariablesOnRestart()
        {
            base.DisposeVariablesOnRestart();
            SavedSpeeds = new();
        }

        /// <inheritdoc/>
        protected override void OnChangingRole(ChangingRoleEventArgs e)
        {
            base.OnChangingRole(e);
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
            Object.Destroy(robinkill.gameObject);
        }

        /// <inheritdoc/>
        protected override void EnablePower(Player player)
        {
            base.EnablePower(player);
            if (!SavedSpeeds.TryGetValue(player, out byte _))
            {
                SavedSpeeds.Add(player, player.GetEffect(EffectType.MovementBoost)?.Intensity ?? 0);
            }
            else
            {
                SavedSpeeds[player] = player.GetEffect(EffectType.MovementBoost)?.Intensity ?? 0;
            }

            player.EnableEffect(EffectType.Invigorated);
            player.GetEffect(EffectType.MovementBoost).Intensity = MovementSpeedIntensity;
        }

        /// <inheritdoc/>
        protected override void DisablePower(Player player)
        {
            base.DisablePower(player);
            player.GetEffect(EffectType.MovementBoost).Intensity = SavedSpeeds[player];
            player.DisableEffect(EffectType.Invigorated);
        }
    }
}
