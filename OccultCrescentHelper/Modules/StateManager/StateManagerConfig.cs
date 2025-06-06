using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.StateManager;

[Title("State Manager Config")]
public class StateManagerConfig : ModuleConfig
{
    [Checkbox]
    [Label("Show State")]
    [Tooltip("Show the current state in the main ui.")]
    public bool ShowDebug { get; set; } = false;
}
