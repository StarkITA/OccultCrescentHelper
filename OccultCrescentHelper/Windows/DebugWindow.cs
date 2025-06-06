using ImGuiNET;
using OccultCrescentHelper.Modules.Debug;
using Ocelot.Modules;
using Ocelot.Windows;

namespace OccultCrescentHelper.Windows;

public class DebugWindow : OcelotWindow
{
    public DebugWindow(Plugin plugin, Config config)
        : base(plugin, config, "OCH Debug")
    { }

    public override void Draw()
    {
        if (!Helpers.IsInOccultCrescent())
        {
            ImGui.TextUnformatted("Not in Occult Crescent zone.");
            return;
        }

        if (plugin.modules.TryGetModule<DebugModule>(out var module))
        {
            module.DrawPanel();
        }
    }
}
