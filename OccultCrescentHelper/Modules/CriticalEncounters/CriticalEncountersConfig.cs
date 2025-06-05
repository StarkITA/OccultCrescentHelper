using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.CriticalEncounters;

[Title("Critical Encounters Config")]
public class CriticalEncountersConfig : ModuleConfig
{
    [Checkbox]
    [Label("Enabled")]
    public bool Enabled { get; set; } = true;
}
