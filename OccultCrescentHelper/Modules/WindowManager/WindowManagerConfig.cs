using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.WindowManager;

[Title("Window Manager Config")]
public class WindowManagerConfig : ModuleConfig
{
    [Checkbox]
    [Label("Open main UI on load")]
    [Tooltip("Opens the main UI when the plugin is loaded")]
    public bool OpenMainOnStartUp { get; set; } = false;

    [Checkbox]
    [Label("Open main ui on enter")]
    [Tooltip("Opens the main ui when you enter an Occult Crescent zone")]
    public bool OpenMainOnEnter { get; set; } = true;

    [Checkbox]
    [Label("Close main ui on exit")]
    [Tooltip("Closes the main ui when you leave an Occult Crescent zone")]
    public bool CloseMainOnExit { get; set; } = true;


    [Checkbox]
    [Label("Open config UI on load")]
    [Tooltip("Opens the config UI when the plugin is loaded")]
    public bool OpenConfigOnStartUp { get; set; } = false;

    [Checkbox]
    [Label("Open config ui on enter")]
    [Tooltip("Opens the config ui when you enter an Occult Crescent zone")]
    public bool OpenConfigOnEnter { get; set; } = false;

    [Checkbox]
    [Label("Close config ui on exit")]
    [Tooltip("Closes the config ui when you leave an Occult Crescent zone")]
    public bool CloseConfigOnExit { get; set; } = true;
}
