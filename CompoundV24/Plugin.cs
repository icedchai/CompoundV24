namespace CompoundV24
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using ColdWaterLibrary.Audio.Features.Helpers;
    using CompoundV24.API.Features.Powers;
    using CompoundV24.EventHandlers;
    using LabApi.Features;
    using LabApi.Loader.Features.Plugins;
    using UserSettings.ServerSpecific;
    using Log = LabApi.Features.Console.Logger;

    /// <summary>
    /// The entrypoint.
    /// </summary>
    public class Plugin : Plugin<Config>
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
        public override string Description => "Adds Compound V";

        /// <inheritdoc/>
        public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

        /// <inheritdoc/>
        public override Version Version => new Version(0, 1, 2);

        /// <inheritdoc/>
        public override void Enable()
        {
            Singleton = this;

            SoundHelper.RegisterSoundGroup(Config.NameToPathForSounds);
            Config.RegisterPowers();

            Config defaultConfig = new Config();
            SoundHelper.RegisterSoundGroup(defaultConfig.NameToPathForSounds);
            defaultConfig = null;

            eventHandlers = new ();
            eventHandlers.SubscribeEvents();

            // SSGroupHeader ssHeader = new SSGroupHeader(Config.SettingHeaderLabel);

            ServerSpecificSettingBase[] settings = new ServerSpecificSettingBase[]
            {
                new SSGroupHeader(Config.SettingHeaderLabel),
                new SSKeybindSetting(Config.SecondaryKeybindId, Config.SecondaryKeybindLabel, UnityEngine.KeyCode.H),
                new SSKeybindSetting(Config.PrimaryKeybindId, Config.PrimaryKeybindLabel, UnityEngine.KeyCode.B),
            };
            ServerSpecificSettingsSync.DefinedSettings ??= new ServerSpecificSettingBase[0];

            IEnumerable<ServerSpecificSettingBase> definedSettings = ServerSpecificSettingsSync.DefinedSettings;
            foreach (var setting in settings)
            {
                definedSettings = definedSettings.Append(setting);
            }

            ServerSpecificSettingsSync.DefinedSettings = definedSettings.ToArray();

            ServerSpecificSettingsSync.SendToAll();
        }

        /// <inheritdoc/>
        public override void Disable()
        {
            Singleton = null;

            eventHandlers.UnsubscribeEvents();

            PowerManager.Instance.UnregisterAll();

            eventHandlers = null;
        }
    }
}
