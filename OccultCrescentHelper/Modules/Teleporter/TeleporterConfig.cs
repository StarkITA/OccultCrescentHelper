using Lumina.Excel.Sheets;
using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.Teleporter;

[Title("Teleporter Config")]
[RequiredPlugin("vnavmesh", "Lifestream")]
public class TeleporterConfig : ModuleConfig
{
    public override string ProviderNamespace => "OccultCrescentHelper.Modules.Teleporter";

    [Checkbox]
    [Label("Mount after teleporting")]
    [Tooltip("The mount to use after teleporting")]
    public bool ShouldMount { get; set; } = true;

    [ExcelSheet(typeof(Mount), nameof(MountProvider))]
    [RenderIf(nameof(ShouldMount))]
    [Label("Mount")]
    [Tooltip("The mount to use after teleporting")]
    public uint Mount { get; set; } = 1;

    [Checkbox]
    [Label("Path to event after teleporting")]
    [Tooltip("Use vnavmesh to head to the next event after teleporting")]
    public bool PathToDestination { get; set; } = false;
}
