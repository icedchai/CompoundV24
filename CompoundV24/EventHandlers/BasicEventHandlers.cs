namespace CompoundV24.EventHandlers
{
    using CompoundV24.Powers;
    using Exiled.API.Features;
    using Exiled.API.Features.Core.UserSettings;
    using Exiled.Events.EventArgs.Player;
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

        private void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase settingBase)
        {
            if (!Player.TryGet(hub, out Player player))
            {
                return;
            }

            if (!PowerManager.Instance.PlayersToPowers.TryGetValue(player, out var powers))
            {
                return;
            }

            Superpower power;
            if (settingBase is SSKeybindSetting keybind && keybind.SettingId == Plugin.Config.PrimaryKeybindId && keybind.SyncIsPressed)
            {
                for (int i = 0; i < powers.Count; i++)
                {
                    if (powers.TryGet(i, out power) && power is not null)
                    {
                        power.OnUsedAbility(player);
                        break;
                    }
                }
            }
            else
            if (settingBase is SSKeybindSetting keybind2 && keybind2.SettingId == Plugin.Config.SecondaryKeybindId && keybind2.SyncIsPressed)
            {
                for (int i = 1; i < powers.Count; i++)
                {
                    if (powers.TryGet(i, out power) && power is not null)
                    {
                        power.OnUsedAbility(player);
                        break;
                    }
                }
            }
        }
    }
}
