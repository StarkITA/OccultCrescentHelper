using System;
using OccultCrescentHelper.ConfigAttributes;
using OccultCrescentHelper.Modules;

namespace OccultCrescentHelper.CriticalEncounters;

[Serializable]
[Title("Critical Encounter Config")]
public class CriticalEncounterConfig : ModuleConfig
{
    [CheckboxConfig]
    [Label("Enabled")]
    public bool Enabled { get; set; } = true;
}
