namespace CompoundV24
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Security.Policy;
    using System.Text;
    using System.Threading.Tasks;
    using CompoundV24.Items;
    using Exiled.API.Interfaces;

    #pragma warning disable SA1600
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        [Description("The Compound V24 CustomItem")]
        public CompoundV CompoundVItem { get; set; } = new ();

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

        public Dictionary<string, List<string>> NameToPathForSounds { get; set; } = new Dictionary<string, List<string>>
        {
            { "burn", new List<string> { @"full/path/to/burn.ogg", @"full/path/to/burn2.ogg" } },
            { "hallway", new List<string> { @"full/path/to/hallway.ogg" } },
            { "gore", new List<string> { @"full/path/to/gore.ogg", @"full/path/to/gore2.ogg" } },
            { "laser", new List<string> { @"full/path/to/laser.ogg", @"full/path/to/laser.ogg" } },
            { "laser_start", new List<string> { @"full/path/to/laser_start.ogg", @"full/path/to/laser_start.ogg" } },
            { "laser_end", new List<string> { @"full/path/to/laser_end.ogg", @"full/path/to/laser_end.ogg" } },
        };
    }
}
