using System.Collections.Generic;
using System.ComponentModel;
using CompoundV24.API.Features.Powers.Superpowers;
using CompoundV24.Items;
using CompoundV24.Powers.Superpowers;

namespace CompoundV24;
#pragma warning disable SA1600
public class Config
{
    public bool IsEnabled { get; set; } = true;

    public bool Debug { get; set; } = false;

    [Description("The message to display to someone when using an ability that is on cooldown.")]
    public string CooldownMessage { get; set; } = "This ability is on cooldown. Seconds remaining: {0}";

    [Description("The Compound V24 CustomItem")]
    public CompoundV CompoundVItem { get; set; } = new ();

    public AntiSupeGun AntiSupeGun { get; set; } = new AntiSupeGun();

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

    public UncontrollableSuperspeed UncontrollableSuperspeed { get; set; } = new ();

    public ControllableSuperspeed ControllableSuperspeed { get; set; } = new ();

    public LaserVision LaserVisionPower { get; set; } = new ();

    public Dictionary<string, List<string>> NameToPathForSounds { get; set; } = new Dictionary<string, List<string>>
    {
        { "hallway", new List<string> { "{labapi_plugin_folder}_icedchqi_vsounds/hallway.ogg" } },
        { "gore", new List<string> { "{labapi_plugin_folder}_icedchqi_vsounds/gore1.ogg", "{labapi_plugin_folder}_icedchqi_vsounds/gore2.ogg" } },
        { "laser", new List<string> { "{labapi_plugin_folder}_icedchqi_vsounds/laser.ogg" } },
        { "laser_start", new List<string> { "{labapi_plugin_folder}_icedchqi_vsounds/laser_start.ogg" } },
        { "laser_end", new List<string> { "{labapi_plugin_folder}_icedchqi_vsounds/laser_end.ogg" } },
    };

    internal void RegisterPowers()
    {
        UncontrollableSuperspeed.Register();
        ControllableSuperspeed.Register();
        LaserVisionPower.Register();
    }
}
