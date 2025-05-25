namespace CompoundV24.API.Features.Powers
{
    using CompoundV24.API.Features.Powers.Interfaces;
    using Exiled.API.Features;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Manages the super powers and who has them.
    /// </summary>
    public class PowerManager
    {
        /// <summary>
        /// Gets the <see cref="PowerManager"/> instance.
        /// </summary>
        public static PowerManager Instance { get; private set; } = new PowerManager();

        /// <summary>
        /// Gets the list of registered powers.
        /// </summary>
        internal List<Superpower> Registered { get; set; } = new List<Superpower>();

        /// <summary>
        /// Gets a lookup table of <see cref="IAbilityPower"/>'s and <see cref="List{T}"/> of <see cref="Player"/>'s with that power enabled.
        /// </summary>
        public Dictionary<IAbilityPower, List<Player>> PowersToEnabledPlayers { get; internal set; }

        public bool PlayerHasPowerEnabled(IAbilityPower power, Player player)
        {
            return PowersToEnabledPlayers.TryGetValue(power, out List<Player> playerList) ? playerList.Contains(player) : false;
        }

        /// <summary>
        /// Gets the list of registered powers.
        /// </summary>
        internal IReadOnlyCollection<Superpower> CompoundVPowers => Registered.Where(p => p.IsCompoundV).ToList();

        /// <summary>
        /// Gets an <see cref="IReadOnlyCollection{Superpower}"/> of <see cref="Superpower"/>'s.
        /// </summary>
        public IReadOnlyCollection<Superpower> Instances => Registered;

        /// <summary>
        /// Gets or sets the lookup table between <see cref="ReferenceHub"/>'s and <see cref="ActiveSuperpower"/>'s.
        /// </summary>
        internal Dictionary<Player, List<Superpower>> PlayersToPowers { get; set; } = new();

        /// <summary>
        /// Unregisters all <see cref="ActiveSuperpower"/>'s.
        /// </summary>
        public void UnregisterAll()
        {
            foreach (Superpower instance in Registered)
            {
                instance.Unregister();
            }
        }
    }
}
