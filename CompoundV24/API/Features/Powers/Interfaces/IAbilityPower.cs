using LabApi.Features.Wrappers;

namespace CompoundV24.API.Features.Powers.Interfaces;

/// <summary>
/// Interface for power that has an ability.
/// </summary>
public interface IAbilityPower
{
    /// <summary>
    /// Called when the player fires their primary power.
    /// </summary>
    /// <param name="player">The player to apply this power to.</param>
    public abstract void OnUsedAbility(Player player);
}
