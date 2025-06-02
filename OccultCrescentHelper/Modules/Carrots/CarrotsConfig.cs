using System;
using OccultCrescentHelper.ConfigAttributes;
using OccultCrescentHelper.Modules;

namespace OccultCrescentHelper.Carrots;

[Serializable]
[Title("Carrots Config")]
public class CarrotsConfig : ModuleConfig
{
    [CheckboxConfig]
    [Label("Enabled")]
    public bool Enabled { get; set; } = true;

    [CheckboxConfig]
    [RenderIf(nameof(Enabled))]
    [Label("Draw line to Carrots")]
    [Tooltip("Render a line from your characters position to nearby carrots.\n * Only carrots in screen space can have lines drawn to them.")]
    public bool DrawLineToCarrots { get; set; } = true;
}
