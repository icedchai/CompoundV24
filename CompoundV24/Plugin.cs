namespace CompoundV24
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using ColdWaterLibrary.Audio.Features.Helpers;
    using CompoundV24.API.Features.Powers;
    using CompoundV24.EventHandlers;
    using Exiled.API.Features.Core.UserSettings;
    using Exiled.CustomItems.API.Features;
    using UserSettings.ServerSpecific;

    /// <summary>
    /// The entrypoint.
    /// </summary>
    public class Plugin : Exiled.API.Features.Plugin<Config>
    {
        /// <summary>
        /// Gets the plugin singleton.
        /// </summary>
        public static Plugin Singleton { get; private set; }

        private static BasicEventHandlers eventHandlers;

        /// <summary>
        /// Gets <see cref="Singleton"/>'s config.
        /// </summary>
        public static new Config Config => Singleton._config;

        private Config _config => base.Config;

        /// <inheritdoc/>
        public override string Author => "icedchqi";

        /// <inheritdoc/>
        public override string Name => "Compound V24";

        /// <inheritdoc/>
        public override string Prefix => "compound_v";

        /// <inheritdoc/>
        public override Version Version => new Version(0, 1, 2);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            base.OnEnabled();
            Singleton = this;

            SoundHelper.RegisterSoundGroup(Config.NameToPathForSounds);
            Config.RegisterPowers();

            Config defaultConfig = new Config();
            SoundHelper.RegisterSoundGroup(defaultConfig.NameToPathForSounds);
            defaultConfig = null;

            eventHandlers = new ();
            eventHandlers.SubscribeEvents();
            CustomItem.RegisterItems(overrideClass: Config);

            // SSGroupHeader ssHeader = new SSGroupHeader(Config.SettingHeaderLabel);

            HeaderSetting header = new HeaderSetting(Config.SettingHeaderLabel);
            IEnumerable<SettingBase> settingBases = new SettingBase[]
            {
                header,
                new KeybindSetting(Config.PrimaryKeybindId, Config.PrimaryKeybindLabel, UnityEngine.KeyCode.Mouse4),
                new KeybindSetting(Config.SecondaryKeybindId, Config.SecondaryKeybindLabel, UnityEngine.KeyCode.Mouse5),
            };
            SettingBase.Register(settingBases);
            SettingBase.SendToAll();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();
            Singleton = null;

            eventHandlers.UnsubscribeEvents();

            CustomItem.UnregisterItems();

            SettingBase.Unregister();

            PowerManager.Instance.UnregisterAll();

            eventHandlers = null;
        }
    }
}
