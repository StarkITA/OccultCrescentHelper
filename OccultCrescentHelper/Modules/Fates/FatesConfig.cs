using System;
using OccultCrescentHelper.ConfigAttributes;
using OccultCrescentHelper.Modules;

namespace OccultCrescentHelper.Fates;

[Serializable]
[Title("Fates Config")]
public class FatesConfig : ModuleConfig
{
    [CheckboxConfig]
    [Label("Enabled")]
    public bool Enabled { get; set; } = true;

    [CheckboxConfig]
    [RenderIf(nameof(Enabled))]
    [Label("Show Demiatma drops")]
    [Tooltip("Show Demiatma drops in the active fate list.")]
    public bool ShowDemiatmaDrops { get; set; } = true;

    [CheckboxConfig]
    [RenderIf(nameof(Enabled))]
    [Label("Show Notes drops")]
    [Tooltip("Show Notes drops in the active fate list.")]
    public bool ShowNoteDrops { get; set; } = true;
}
