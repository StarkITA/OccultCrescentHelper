using System;
using OccultCrescentHelper.ConfigAttributes;
using OccultCrescentHelper.Modules;

namespace OccultCrescentHelper.Currency;

[Serializable]
[Title("Currency Config")]
public class CurrencyConfig : ModuleConfig
{
    [CheckboxConfig]
    [Label("Enabled")]
    public bool Enabled { get; set; } = true;
}
