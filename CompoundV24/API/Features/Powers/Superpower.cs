namespace CompoundV24.API.Features.Powers
{
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    /// <summary>
    /// The abstract superpower.
    /// </summary>
    public abstract class Superpower
    {
        /// <summary>
        /// Gets the name of this power.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this superpower is to be added when Compound V is used.
        /// </summary>
        public virtual bool IsCompoundV { get; } = false;

        /// <summary>
        /// Gets the description of this power.
        /// </summary>
        public virtual string Description { get; } = "No description provided.";

        /// <summary>
        /// Gets the <see cref="PowerManager"/> singleton.
        /// </summary>
        protected static PowerManager PowerManager => PowerManager.Instance;

        /// <summary>
        /// Gets or sets the special damage multipliers for this superpower. Will be multiplied ON TOP of the <see cref="UniversalDamageMultiplier"/>.
        /// </summary>
        protected virtual Dictionary<DamageType, float> DamageMultipliers { get; set; } = new()
        {
            { DamageType.Unknown, 1f },
        };

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

            Log.Debug($"Registered power {GetType().Name}");
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
            Exiled.Events.Handlers.Server.RestartingRound += DisposeVariablesOnRestart;
            Exiled.Events.Handlers.Player.Hurting += OnInternalHurting;
            Exiled.Events.Handlers.Player.ChangingRole += OnInternalChangingRole;
        }

        /// <summary>
        /// Unsubscribes events for this power.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Server.RestartingRound -= DisposeVariablesOnRestart;
            Exiled.Events.Handlers.Player.Hurting -= OnInternalHurting;
            Exiled.Events.Handlers.Player.ChangingRole -= OnInternalChangingRole;
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
            if (player is null || !PowerManager.PlayersToPowers.TryGetValue(player, out var powers))
            {
                PowerManager.PlayersToPowers.Add(player, new List<Superpower> { this });
            }
            else
            {
                PowerManager.PlayersToPowers[player].Add(this);
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

        private void OnInternalHurting(HurtingEventArgs e)
        {
            if (e.Player is null || !Check(e.Player))
            {
                return;
            }

            e.Amount *= UniversalDamageMultiplier;

            if (DamageMultipliers.TryGetValue(e.DamageHandler.Type, out float multiplier))
            {
                e.Amount *= multiplier;
            }

            OnHurting(e);
        }

        private void OnInternalChangingRole(ChangingRoleEventArgs e)
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
        protected virtual void OnHurting(HurtingEventArgs e)
        {
        }

        /// <summary>
        /// Ran when someone with this superpower has their role changed.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnChangingRole(ChangingRoleEventArgs e)
        {
        }
    }
}
