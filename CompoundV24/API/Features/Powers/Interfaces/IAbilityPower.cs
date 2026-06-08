using LabApi.Features.Wrappers;
using PlayerRoles.Subroutines;

namespace CompoundV24.API.Features.Powers.Interfaces;

/// <summary>
/// Interface for power that has an ability.
/// </summary>
public interface IAbilityPower
{
    /// <summary>
    /// Gets the cooldown on this ability.
    /// </summary>
    public abstract float Cooldown { get; internal set; }

    /// <summary>
    /// Called when the player fires this power.
    /// </summary>
    /// <param name="player">The player to apply this power to.</param>
    public abstract void OnUsedAbility(Player player);
}
