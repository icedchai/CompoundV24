namespace CompoundV24.EventHandlers
{
    using System.Collections.Generic;
    using System.Linq;
    using CompoundV24.API.Features.Powers;
    using CompoundV24.API.Features.Powers.Interfaces;
    using LabApi.Features.Wrappers;
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
            LabApi.Events.Handlers.ServerEvents.RoundRestarted += ResetPowermanager;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnSettingValueReceived;
        }

        /// <summary>
        /// Unegisters the events.
        /// </summary>
        public void UnsubscribeEvents()
        {
            LabApi.Events.Handlers.ServerEvents.RoundRestarted -= ResetPowermanager;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= OnSettingValueReceived;
        }

        private void ResetPowermanager()
        {
            PowerManager.Instance.PlayersToPowers = new ();
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
            Player player = Player.Get(hub);
            if (player == null || hub == null)
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
