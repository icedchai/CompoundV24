using System.Collections.Generic;
using CompoundV24.API.Features.Powers.Interfaces;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using PlayerRoles.Subroutines;

namespace CompoundV24.API.Features.Powers.Superpowers;

/// <summary>
/// Toggleable superpower.
/// </summary>
public abstract class ToggleablePower : Superpower, IAbilityPower
{
    /// <inheritdoc/>
    public float Cooldown { get; set; } = 2.5f;

    /// <summary>
    /// Gets the <see cref="AbilityCooldown"/>s for this power.
    /// </summary>
    public Dictionary<Player, AbilityCooldown> AbilityCooldown { get; private set; } = new Dictionary<Player, AbilityCooldown>();

    /// <summary>
    /// Gets or sets the lookup table between <see cref="Player"/>'s and a value indicating whether they have the power enabled.
    /// </summary>
    protected List<Player> EnabledPlayers { get; set; } = new ();

    /// <summary>
    /// Checks whether the player has the power enabled.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <returns>A value indicating whether <paramref name="player"/> has the power enabled.</returns>
    public bool PlayerHasPowerEnabled(Player player)
    {
        return EnabledPlayers.Contains(player);
    }

    /// <inheritdoc/>
    public virtual void OnUsedAbility(Player player)
    {
        if (!AbilityCooldown.TryGetValue(player, out AbilityCooldown cooldown))
        {
            cooldown = new AbilityCooldown();
            AbilityCooldown.Add(player, cooldown);
        }

        if (cooldown.IsReady)
        {
            cooldown.Trigger(Cooldown);
            TogglePower(player);
        }
        else
        {
            player.SendHint(string.Format(Plugin.Config.CooldownMessage, cooldown.Remaining));
        }
    }

    /// <inheritdoc/>
    protected override void DisposeVariablesOnRestart()
    {
        base.DisposeVariablesOnRestart();
        EnabledPlayers.Clear();
        AbilityCooldown.Clear();
    }

    /// <inheritdoc/>
    protected override void OnChangingRole(PlayerChangingRoleEventArgs e)
    {
        base.OnChangingRole(e);
        EnabledPlayers.Remove(e.Player);
    }

    /// <inheritdoc/>
    protected override void RemoveProperties(Player player)
    {
        base.RemoveProperties(player);
        if (PlayerHasPowerEnabled(player))
        {
            TogglePower(player);
        }
    }

    /// <summary>
    /// Toggles the power for <paramref name="player"/>.
    /// </summary>
    /// <param name="player">The player for whom to toggle the power.</param>
    protected virtual void TogglePower(Player player)
    {
        if (PlayerHasPowerEnabled(player))
        {
            DisablePower(player);
        }
        else
        {
            EnablePower(player);
        }
    }

    /// <summary>
    /// Enables the power on <paramref name="player"/>.
    /// </summary>
    /// <param name="player">The player to enable the power on.</param>
    protected virtual void EnablePower(Player player)
    {
        EnabledPlayers.Add(player);
    }

    /// <summary>
    /// Disables the power on <paramref name="player"/>.
    /// </summary>
    /// <param name="player">The player to disable the power on.</param>
    protected virtual void DisablePower(Player player)
    {
        EnabledPlayers.Remove(player);
    }
}
