using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.Treasure;

[Title("Treasure Config")]
public class TreasureConfig : ModuleConfig
{
    [Checkbox]
    [Label("Enabled")]
    public bool Enabled { get; set; } = true;

    [Checkbox]
    [DependsOn(nameof(Enabled))]
    [Label("Draw lines to bronze coffers")]
    [Tooltip("Render a line from your characters position to nearby bronze coffers.")]
    public bool DrawLineToBronzeChests { get; set; } = true;
    public bool ShouldDrawLineToBronzeChests => IsPropertyEnabled(nameof(DrawLineToBronzeChests));

    [Checkbox]
    [DependsOn(nameof(Enabled))]
    [Label("Draw lines to silver coffers")]
    [Tooltip("Render a line from your characters position to nearby silver coffers.")]
    public bool DrawLineToSilverChests { get; set; } = true;
    public bool ShouldDrawLineToSilverChests => IsPropertyEnabled(nameof(DrawLineToSilverChests));

    [Checkbox]
    [Experimental]
    [Illegal]
    [RequiredPlugin("vnavmesh", "Lifestream")]
    [DependsOn(nameof(Enabled))]
    [Label("Enable Treasure Hunt button")]
    public bool EnableTreasureHunt { get; set; } = false;
    public bool ShouldEnableTreasureHunt => IsPropertyEnabled(nameof(EnableTreasureHunt));

    [FloatRange(10f, 100f)]
    [DependsOn(nameof(Enabled), nameof(EnableTreasureHunt))]
    [Label("Chest detection range")]
    [Tooltip("The distance from a node you have to be to assume it isn't there.\nThese can be variable depending on when the server sends you the gameobject instance.\nA lower value will be more reliable but will make the hunt take longer.")]
    public float ChestDetectionRange { get; set; } = 75f;

    [IntRange(1, 28)]
    [Experimental]
    [DependsOn(nameof(Enabled), nameof(EnableTreasureHunt))]
    [Label("Max area level")]
    [Tooltip("The max level of mob you are willing to hunt for treasure near")]
    public int MaxLevel { get; set; } = 28;
}
