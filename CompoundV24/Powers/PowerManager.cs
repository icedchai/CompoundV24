namespace CompoundV24.Powers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Exiled.API.Features;

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
        /// Gets the list of registered powers.
        /// </summary>
        internal IReadOnlyCollection<Superpower> CompoundVPowers => Registered.Where(p => p.IsCompoundV).ToList();

        /// <summary>
        /// Gets an <see cref="IReadOnlyCollection{Superpower}"/> of <see cref="Superpower"/>'s.
        /// </summary>
        public IReadOnlyCollection<Superpower> Instances => Registered;

        /// <summary>
        /// Gets or sets the lookup table between <see cref="ReferenceHub"/>'s and <see cref="Superpower"/>'s.
        /// </summary>
        internal Dictionary<Player, List<Superpower>> PlayersToPowers { get; set; } = new ();

        /// <summary>
        /// Unregisters all <see cref="Superpower"/>'s.
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
