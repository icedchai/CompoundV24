namespace CompoundV24.Powers.Interfaces
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using System.Collections.Generic;

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
}
