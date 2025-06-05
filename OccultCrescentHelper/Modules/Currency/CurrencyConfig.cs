using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.Currency;

[Title("Currency Config")]
public class CurrencyConfig : ModuleConfig
{
    [Checkbox]
    [Label("Enabled")]
    public bool Enabled { get; set; } = true;
}
