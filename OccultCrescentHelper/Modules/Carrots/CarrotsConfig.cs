using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.Carrots;

[Title("Carrots Config")]
public class CarrotsConfig : ModuleConfig
{
    [Checkbox]
    [Label("Enabled")]
    public bool Enabled { get; set; } = true;

    [Checkbox]
    [DependsOn(nameof(Enabled))]
    [Label("Draw line to Carrots")]
    [Tooltip("Render a line from your characters position to nearby carrots.")]
    public bool DrawLineToCarrots { get; set; } = true;
    public bool ShouldDrawLineToCarrots => IsPropertyEnabled(nameof(DrawLineToCarrots));
}
