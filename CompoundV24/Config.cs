﻿namespace CompoundV24
{
    using CompoundV24.Items;
    using CompoundV24.Powers.Superpowers;
    using Exiled.API.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel;

#pragma warning disable SA1600
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        [Description("The Compound V24 CustomItem")]
        public CompoundV CompoundVItem { get; set; } = new();

        [Description("The centered text (header) of the category.")]
        public string SettingHeaderLabel { get; set; } = "Compound V";

        [Description("The unique id of the setting.")]
        public int PrimaryKeybindId { get; set; } = 1100;

        [Description("The keybind label.")]
        public string PrimaryKeybindLabel { get; set; } = "Superpower primary ability.";

        [Description("The unique id of the setting.")]
        public int SecondaryKeybindId { get; set; } = 1101;

        [Description("The keybind label.")]
        public string SecondaryKeybindLabel { get; set; } = "Superpower secondary ability.";

        public UncontrollableSuperspeed UncontrollableSuperspeed { get; set; } = new();

        public ControllableSuperspeed ControllableSuperspeed { get; set; } = new();

        public LaserVision LaserVisionPower { get; set; } = new();

        internal void RegisterPowers()
        {
            UncontrollableSuperspeed.Register();
            ControllableSuperspeed.Register();
            LaserVisionPower.Register();
        }

        public Dictionary<string, List<string>> NameToPathForSounds { get; set; } = new Dictionary<string, List<string>>
        {
            { "hallway", new List<string> { "{defaultpath}_icedchqi_vsounds/hallway.ogg" } },
            { "gore", new List<string> { "{defaultpath}_icedchqi_vsounds/gore1.ogg", "{defaultpath}_icedchqi_vsounds/gore2.ogg" } },
            { "laser", new List<string> { "{defaultpath}_icedchqi_vsounds/laser.ogg" } },
            { "laser_start", new List<string> { "{defaultpath}_icedchqi_vsounds/laser_start.ogg" } },
            { "laser_end", new List<string> { "{defaultpath}_icedchqi_vsounds/laser_end.ogg" } },
        };
    }
}
