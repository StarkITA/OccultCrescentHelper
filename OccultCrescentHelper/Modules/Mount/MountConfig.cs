using Ocelot.Config.Attributes;
using Ocelot.Modules;
using ExcelMount = Lumina.Excel.Sheets.Mount;

namespace OccultCrescentHelper.Modules.Mount;

[Title("Mount Config")]
public class MountConfig : ModuleConfig
{
    public override string ProviderNamespace => "OccultCrescentHelper.Modules.Mount";

    [ExcelSheet(typeof(ExcelMount), nameof(MountProvider))]
    [Label("Mount")]
    [Tooltip("The mount to use.")]
    public uint Mount { get; set; } = 1;

    [Checkbox]
    [Experimental]
    [Label("Stay mounted while out of combat")]
    [Tooltip("Overrides")]
    public bool MaintainMount { get; set; } = false;

    [Checkbox]
    [Label("Use Mount Roulette")]
    [Tooltip("Overrides configured mount and uses roulette instead")]
    public bool MountRoulette { get; set; } = false;
}
