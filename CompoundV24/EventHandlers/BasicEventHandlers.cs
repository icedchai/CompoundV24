namespace CompoundV24.EventHandlers
{
    using CompoundV24.API.Features.Powers.Interfaces;
    using CompoundV24.API.Features.Powers;
    using Exiled.API.Features;
    using Exiled.API.Features.Core.UserSettings;
    using Exiled.Events.EventArgs.Player;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UserSettings.ServerSpecific;

    /// <summary>
    /// Basic event handlers.
    /// </summary>
    public class BasicEventHandlers
    {
        /// <summary>
        /// Registers the events.
        /// </summary>
        public void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Server.RestartingRound += ResetPowermanager;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnSettingValueReceived;
        }

        /// <summary>
        /// Unegisters the events.
        /// </summary>
        public void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Server.RestartingRound -= ResetPowermanager;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= OnSettingValueReceived;
        }

        private void OnVerified(VerifiedEventArgs e)
        {
            if (e.Player is null)
            {
                return;
            }

            SettingBase.SendToPlayer(e.Player);
        }

        private void ResetPowermanager()
        {
            PowerManager.Instance.PlayersToPowers = new();
        }

        private void AttemptAbility(int index, Player player, List<IAbilityPower> powers)
        {
            for (int i = index; i < powers.Count; i++)
            {
                if (powers.TryGet(i, out IAbilityPower power))
                {
                    power.OnUsedAbility(player);
                    break;
                }
            }
        }

        private void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase settingBase)
        {
            if (!Player.TryGet(hub, out Player player))
            {
                return;
            }

            if (!PowerManager.Instance.PlayersToPowers.TryGetValue(player, out List<Superpower> powers))
            {
                return;
            }

            List<IAbilityPower> abilityPowers = powers.Where(p => p is IAbilityPower).ToList().ConvertAll(p => p as IAbilityPower);

            if (settingBase is SSKeybindSetting keybind && keybind.SettingId == Plugin.Config.PrimaryKeybindId && keybind.SyncIsPressed)
            {
                AttemptAbility(0, player, abilityPowers);
            }
            else if (settingBase is SSKeybindSetting keybind2 && keybind2.SettingId == Plugin.Config.SecondaryKeybindId && keybind2.SyncIsPressed)
            {
                AttemptAbility(1, player, abilityPowers);
            }
        }
    }
}
