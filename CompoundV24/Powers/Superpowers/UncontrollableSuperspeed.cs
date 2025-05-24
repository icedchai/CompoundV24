namespace CompoundV24.Powers.Superpowers
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using MEC;

    /// <summary>
    /// Controllable superspeed.
    /// </summary>
    public class UncontrollableSuperspeed : ControllableSuperspeed
    {
        /// <inheritdoc/>
        public override string Name { get; } = "superspeed_uncontrolled";

        public float ChanceOfHeartAttackAfterRunning { get; set; } = 30f;

        public float HeartAttackLength { get; set; } = 10f;

        public float SuperspeedBurstLength { get; set; } = 10f;

        /// <inheritdoc/>
        public override bool IsCompoundV => true;

        /// <inheritdoc/>
        public void OnUsedAbility(Player player)
        {
            base.OnUsedAbility(player);

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
