using System;
using OccultCrescentHelper.ConfigAttributes;
using OccultCrescentHelper.Modules;

[Serializable]
[Title("Teleporter Config")]
public class TeleporterConfig : ModuleConfig
{
    [CheckboxConfig]
    [Label("Should Mount")]
    [Tooltip("The mount to use after teleporting")]
    public bool ShouldMount { get; set; } = true;

    [MountConfig]
    [Label("Mount")]
    [Tooltip("The mount to use after teleporting")]
    public uint Mount { get; set; } = 1;
}
