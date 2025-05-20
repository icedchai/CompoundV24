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
            { "shell", new List<string> { @"full/path/to/shell1.ogg", @"full/path/to/shell2.ogg" } },
        };
    }
}
