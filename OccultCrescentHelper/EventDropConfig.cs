using System;
using OccultCrescentHelper.ConfigAttributes;
using OccultCrescentHelper.Modules;

[Serializable]
[Title("Event Drop Config")]
public class EventDropConfig : ModuleConfig
{
    [CheckboxConfig]
    [Label("Show Demiatma drops")]
    [Tooltip("Show Demiatma drops in the active fate/ce list.")]
    public bool ShowDemiatmaDrops { get; set; } = true;

    [CheckboxConfig]
    [Label("Show Notes drops")]
    [Tooltip("Show Notes drops in the active fate/ce list.")]
    public bool ShowNoteDrops { get; set; } = true;

    [CheckboxConfig]
    [Label("Show Soul Shard drops")]
    [Tooltip("Show Soul Shard drops in the active ce list.")]
    public bool ShowSoulShardDrops { get; set; } = true;
}
