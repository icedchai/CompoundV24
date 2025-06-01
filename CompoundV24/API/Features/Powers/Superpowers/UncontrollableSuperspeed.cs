namespace CompoundV24.Powers.Superpowers
{
    using CompoundV24.API.Features.Powers.Superpowers;
    using CustomPlayerEffects;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using MEC;

    /// <summary>
    /// Controllable superspeed.
    /// </summary>
    public class UncontrollableSuperspeed : ControllableSuperspeed
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "superspeed_uncontrolled";

        /// <summary>
        /// Gets or sets the chance of a <see cref="CardiacArrest"/> after a player is done running.
        /// </summary>
        public float ChanceOfHeartAttackAfterRunning { get; set; } = 30f;

        /// <summary>
        /// Gets or sets the length of the <see cref="CardiacArrest"/> after running, if it is rolled.
        /// </summary>
        public float HeartAttackLength { get; set; } = 10f;

        /// <summary>
        /// Gets or sets the length of a superspeed burst in seconds.
        /// </summary>
        public float SuperspeedBurstLength { get; set; } = 10f;

        /// <inheritdoc/>
        public override bool IsCompoundV { get; set; } = true;

        /// <inheritdoc/>
        public override void OnUsedAbility(Player player)
        {
            if (PlayerHasPowerEnabled(player))
            {
                return;
            }
            else
            {
                TogglePower(player);
                Timing.CallDelayed(SuperspeedBurstLength, () =>
                {
                    if (!PlayerHasPowerEnabled(player))
                    {
                        return;
                    }

                    TogglePower(player);
                    if (UnityEngine.Random.Range(0f, 100f) < ChanceOfHeartAttackAfterRunning)
                    {
                        player.EnableEffect(EffectType.CardiacArrest, HeartAttackLength);
                        Timing.CallDelayed(HeartAttackLength, () =>
                        {
                            if (Check(player))
                            {
                                player.DisableEffect(EffectType.CardiacArrest);
                            }
                        });
                    }
                });
            }
        }
    }
}
