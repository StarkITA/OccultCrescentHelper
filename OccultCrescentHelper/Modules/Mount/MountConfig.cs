using Ocelot.Config.Attributes;
using Ocelot.Modules;
using ExcelMount = Lumina.Excel.Sheets.Mount;

namespace OccultCrescentHelper.Modules.Mount;

[Title("modules.mount.title")]
public class MountConfig : ModuleConfig
{
    public override string ProviderNamespace => "OccultCrescentHelper.Modules.Mount";

    [ExcelSheet(typeof(ExcelMount), nameof(MountProvider))]
    [Label("modules.mount.mount.label")]
    [Tooltip("modules.mount.mount.tooltip")]
    public uint Mount { get; set; } = 1;

    [Checkbox]
    [Experimental]
    [Label("modules.mount.maintain.label")]
    [Tooltip("modules.mount.maintain.tooltip")]
    public bool MaintainMount { get; set; } = false;

    [Checkbox]
    [Label("modules.mount.roulette.label")]
    [Tooltip("modules.mount.roulette.tooltip")]
    public bool MountRoulette { get; set; } = false;
}
