namespace CompoundV24.API.Features.Powers.Superpowers
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using MEC;
    using UnityEngine;

    /// <summary>
    /// The shrink and grow superpower.
    /// </summary>
    public class TermiteShrinker : ToggleablePower
    {
        /// <inheritdoc/>
        public override string Name => "termite_power";

        /// <inheritdoc/>
        public override bool IsCompoundV => true;

        private Dictionary<Player, float> SavedScales { get; set; } = new Dictionary<Player, float>();

        /// <summary>
        /// Gets or sets a value indicating whether to size the player according to <see cref="Scale"/> relative or absolute.
        /// </summary>
        public bool ScaleRelative { get; set; } = false;

        /// <summary>
        /// Gets or sets a scale factor to use.
        /// </summary>
        public float Scale { get; set; } = 0.3f;

        /// <summary>
        /// Gets or sets the amount of damage to do to all players when the player unshrinks.
        /// </summary>
        public float DamageToOthers { get; set; } = 150f;

        /// <inheritdoc/>
        protected override void DisposeVariablesOnRestart()
        {
            base.DisposeVariablesOnRestart();
            SavedScales.Clear();
        }

        /// <inheritdoc/>
        protected override void OnChangingRole(ChangingRoleEventArgs e)
        {
            base.OnChangingRole(e);
            e.Player.Scale = Vector3.one;
        }

        /// <inheritdoc/>
        protected override void EnablePower(Player player)
        {
            base.EnablePower(player);
            if (!SavedScales.TryGetValue(player, out float scale))
            {
                SavedScales.Add(player, player.Scale.y);
            }
            else
            {
                SavedScales[player] = player.Scale.y;
            }

            Timing.RunCoroutine(Shrink(player));
        }

        /// <inheritdoc/>
        protected override void DisablePower(Player player)
        {
            base.DisablePower(player);
            Timing.RunCoroutine(Grow(player));
        }

        private IEnumerator<float> Grow(Player player)
        {
            float targetScale = SavedScales[player];
            while (player.Scale.y < targetScale)
            {
                player.Scale *= 0.99f;
                yield return Timing.WaitForOneFrame;
            }

            while (player.Scale.y > targetScale)
            {
                player.Scale *= 1.01f;
                yield return Timing.WaitForOneFrame;
            }

            yield break;
        }

        private IEnumerator<float> Shrink(Player player)
        {
            float targetScale = ScaleRelative ? player.Scale.y * Scale : Scale;
            while (player.Scale.y > targetScale)
            {
                player.Scale *= 0.99f;
                yield return Timing.WaitForOneFrame;
            }

            while (player.Scale.y < targetScale)
            {
                player.Scale *= 1.01f;
                yield return Timing.WaitForOneFrame;
            }

            yield break;
        }
    }
}
