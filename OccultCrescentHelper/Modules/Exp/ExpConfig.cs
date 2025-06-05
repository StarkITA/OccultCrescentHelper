using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.Exp;

[Title("Exp Config")]
public class ExpConfig : ModuleConfig
{
    [Checkbox]
    [Label("Enabled")]
    public bool Enabled { get; set; } = true;
}
