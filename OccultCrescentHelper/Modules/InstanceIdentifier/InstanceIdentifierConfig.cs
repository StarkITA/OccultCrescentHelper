using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.InstanceIdentifier;

[Title("Instance Identifier Config")]
public class InstanceIdentifierConfig : ModuleConfig
{
    [Checkbox]
    [Label("Show")]
    [Tooltip("This is caught when the zone initialises, and will be unknown if the plugin is enabled/reloaded while in the zone")]
    public bool Show { get; set; } = true;
}
