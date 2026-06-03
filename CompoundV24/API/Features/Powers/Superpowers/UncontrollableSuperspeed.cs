using CompoundV24.API.Features.Powers.Superpowers;
using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using MEC;

namespace CompoundV24.Powers.Superpowers;

/// <summary>
/// Uncontrollable superspeed.
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
    public float HeartAttackLength { get; set; } = 2f;

    /// <summary>
    /// Gets or sets the length of a superspeed burst in seconds.
    /// </summary>
    public float SuperspeedBurstLength { get; set; } = 5f;

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
                    player.EnableEffect<CardiacArrest>(1, HeartAttackLength);
                    Timing.CallDelayed(HeartAttackLength, () =>
                    {
                        if (Check(player))
                        {
                            player.DisableEffect<CardiacArrest>();
                        }
                    });
                }
            });
        }
    }
}
