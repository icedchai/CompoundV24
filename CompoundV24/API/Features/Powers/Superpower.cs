using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerStatsSystem;
using System.Collections.Generic;

namespace CompoundV24.API.Features.Powers;

/// <summary>
/// The abstract superpower.
/// </summary>
public abstract class Superpower
{
    /// <summary>
    /// Gets the name of this power.
    /// </summary>
    public abstract string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this superpower is to be obtainable via Compound-V.
    /// </summary>
    public virtual bool IsCompoundV { get; set; } = false;

    /// <summary>
    /// Gets or sets the description of this power.
    /// </summary>
    public virtual string Description { get; set; } = "No description provided.";

    /// <summary>
    /// Gets the <see cref="PowerManager"/> singleton.
    /// </summary>
    protected static PowerManager PowerManager => PowerManager.Instance;

    /// <summary>
    /// Gets or sets the damage multiplier that will be applied to all damage for this superpower.
    /// </summary>
    protected virtual float UniversalDamageMultiplier { get; set; } = 1f;

    /// <summary>
    /// Checks whether the given <see cref="ReferenceHub"/> has this power.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <returns>A value indicating whether a player has this power.</returns>
    public bool Check(Player player)
    {
        if (player is null || !PowerManager.PlayersToPowers.TryGetValue(player, out List<Superpower> powers))
        {
            return false;
        }

        return powers.Contains(this);
    }

    /// <summary>
    /// Registers this power to the <see cref="CompoundV24.Powers.PowerManager"/> singleton.
    /// </summary>
    public void Register()
    {
        if (PowerManager.Registered.Contains(this))
        {
            return;
        }

        SubscribeEvents();
        PowerManager.Registered.Add(this);

        Logger.Debug($"Registered power {GetType().Name}");
    }

    /// <summary>
    /// Unregisters this power from the <see cref="CompoundV24.Powers.PowerManager"/> singleton.
    /// </summary>
    public void Unregister()
    {
        UnsubscribeEvents();
        PowerManager.Registered.Remove(this);
    }

    /// <summary>
    /// Subscribes events for this power.
    /// </summary>
    protected virtual void SubscribeEvents()
    {
        LabApi.Events.Handlers.ServerEvents.RoundRestarted += DisposeVariablesOnRestart;
        LabApi.Events.Handlers.PlayerEvents.Hurting += OnInternalHurting;
        LabApi.Events.Handlers.PlayerEvents.ChangingRole += OnInternalChangingRole;
    }

    /// <summary>
    /// Unsubscribes events for this power.
    /// </summary>
    protected virtual void UnsubscribeEvents()
    {
        LabApi.Events.Handlers.ServerEvents.RoundRestarted -= DisposeVariablesOnRestart;
        LabApi.Events.Handlers.PlayerEvents.Hurting -= OnInternalHurting;
        LabApi.Events.Handlers.PlayerEvents.ChangingRole -= OnInternalChangingRole;
    }

    /// <summary>
    /// Applies the appropriate properties to the player.
    /// </summary>
    /// <param name="player">The player to give this to.</param>
    protected virtual void ApplyProperties(Player player)
    {
    }

    /// <summary>
    /// Removes the appropriate properties from the player.
    /// </summary>
    /// <param name="player">The player to give this to.</param>
    protected virtual void RemoveProperties(Player player)
    {
    }

    /// <summary>
    /// Gives a power to a <see cref="Player"/>.
    /// </summary>
    /// <param name="player">The <see cref="Player"/> to give this power to.</param>
    public void Grant(Player player)
    {
        if (player == null)
        {
            return;
        }

        if (!PowerManager.PlayersToPowers.TryGetValue(player, out var powers))
        {
            PowerManager.PlayersToPowers.Add(player, new List<Superpower> { this });
        }
        else
        {
            powers.Add(this);
        }

        InternalApplyProperties(player);
    }

    private void InternalApplyProperties(Player player)
    {
        if (player is null)
        {
            return;
        }

        ApplyProperties(player);
    }

    /// <summary>
    /// Revokes a power from a <see cref="Player"/>.
    /// </summary>
    /// <param name="player">The <see cref="Player"/> to give this power to.</param>
    public void Revoke(Player player)
    {
        if (player is null || !PowerManager.PlayersToPowers.TryGetValue(player, out var powers))
        {
            return;
        }
        else if (PowerManager.PlayersToPowers[player].Contains(this))
        {
            PowerManager.PlayersToPowers[player].Remove(this);
            InternalRemoveProperties(player);
        }
    }

    /// <summary>
    /// Dispose of game-related variables on round restart. Anything involving <see cref="Player"/>'s should be involved here.
    /// </summary>
    protected virtual void DisposeVariablesOnRestart()
    {
    }

    private void InternalRemoveProperties(Player player)
    {
        if (player is null)
        {
            return;
        }

        RemoveProperties(player);
    }

    private void OnInternalHurting(PlayerHurtingEventArgs e)
    {
        if (e.Player is null || !Check(e.Player))
        {
            return;
        }

        if (e.DamageHandler is StandardDamageHandler damageHandler)
        {
            damageHandler.Damage *= UniversalDamageMultiplier;
        }

        OnHurting(e);
    }

    private void OnInternalChangingRole(PlayerChangingRoleEventArgs e)
    {
        if (e.Player is null || !Check(e.Player))
        {
            return;
        }

        Revoke(e.Player);

        OnChangingRole(e);
    }

    /// <summary>
    /// Ran when someone with this superpower is damaged.
    /// </summary>
    /// <param name="e">The event args.</param>
    protected virtual void OnHurting(PlayerHurtingEventArgs e)
    {
    }

    /// <summary>
    /// Ran when someone with this superpower has their role changed.
    /// </summary>
    /// <param name="e">The event args.</param>
    protected virtual void OnChangingRole(PlayerChangingRoleEventArgs e)
    {
    }
}
