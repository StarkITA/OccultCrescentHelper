using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.Fates;

[Title("Fates Config")]
public class FatesConfig : ModuleConfig
{
    [Checkbox]
    [Label("Enabled")]
    public bool Enabled { get; set; } = true;
}
