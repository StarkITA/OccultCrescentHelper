using Lumina.Excel.Sheets;
using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.Teleporter;

[Title("Teleporter Config")]
public class TeleporterConfig : ModuleConfig
{
    public override string ProviderNamespace => "OccultCrescentHelper.Modules.Teleporter";

    [Checkbox]
    [RequiredPlugin("Lifestream")]
    [Label("Mount after teleporting")]
    [Tooltip("The mount to use after teleporting")]
    public bool ShouldMount { get; set; } = true;

    [ExcelSheet(typeof(Mount), nameof(MountProvider))]
    [RequiredPlugin("Lifestream")]
    [RenderIf(nameof(ShouldMount))]
    [Label("Mount")]
    [Tooltip("The mount to use after teleporting")]
    public uint Mount { get; set; } = 1;

    [Checkbox]
    [RequiredPlugin("vnavmesh")]
    [Label("Path to event after teleporting")]
    [Tooltip("Use vnavmesh to head to the next event after teleporting")]
    public bool PathToDestination { get; set; } = false;

    [Checkbox]
    [Label("Return to base after fate")]
    [Tooltip("Use Occult Return upon finishing an fate to return to base camp")]
    public bool ReturnAfterFate { get; set; } = false;

    [Checkbox]
    [Label("Return to base after critical encounter")]
    [Tooltip("Use Occult Return upon finishing an critical encounter to return to base camp")]
    public bool ReturnAfterCritcalEncounter { get; set; } = false;

    [Checkbox]
    [RequiredPlugin("vnavmesh")]
    [Label("Approach aetheryte after returning")]
    [Tooltip("Walk to the aetheryte after returning to base camp\n * This only works when returning via the plugin after a fate or critical encounter")]
    public bool ApproachAetheryte { get; set; } = false;
}
