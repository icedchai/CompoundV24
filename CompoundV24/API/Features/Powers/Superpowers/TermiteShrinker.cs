namespace CompoundV24.API.Features.Powers.Superpowers
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using MEC;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using UnityEngine;

    public class TermiteShrinker : ToggleablePower
    {
        /// <inheritdoc/>
        public override string Name => "termite_power";

        public override bool IsCompoundV => true;

        private Dictionary<Player, float> SavedScales { get; set; } = new Dictionary<Player, float>();

        public bool ScaleRelative { get; set; } = false;

        public float Scale { get; set; } = 0.3f;

        public float DamageToOthers { get; set; } = 150f;

        protected override void DisposeVariablesOnRestart()
        {
            base.DisposeVariablesOnRestart();
            SavedScales.Clear();
        }

        protected override void OnChangingRole(ChangingRoleEventArgs e)
        {
            base.OnChangingRole(e);
            e.Player.Scale = Vector3.one;
        }

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
