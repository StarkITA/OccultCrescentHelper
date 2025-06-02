using System;
using OccultCrescentHelper.ConfigAttributes;
using OccultCrescentHelper.Modules;

namespace OccultCrescentHelper.Treasure;

[Serializable]
[Title("Treasure Config")]
public class TreasureConfig : ModuleConfig
{
    [CheckboxConfig]
    [Label("Enabled")]
    public bool Enabled { get; set; } = true;

    [CheckboxConfig]
    [RenderIf(nameof(Enabled))]
    [Label("Draw lines to bronze coffers")]
    [Tooltip("Render a line from your characters position to nearby bronze coffers.\n * Only coffers in screen space can have lines drawn to them.")]
    public bool DrawLineToBronzeChests { get; set; } = true;

    [CheckboxConfig]
    [RenderIf(nameof(Enabled))]
    [Label("Draw lines to silver coffers")]
    [Tooltip("Render a line from your characters position to nearby silver coffers.\n * Only coffers in screen space can have lines drawn to them.")]
    public bool DrawLineToSilverChests { get; set; } = true;
}
