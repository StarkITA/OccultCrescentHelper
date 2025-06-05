using ImGuiNET;
using Ocelot.Modules;
using Ocelot.Windows;

namespace OccultCrescentHelper.Windows;

[OcelotMainWindow]
public class MainWindow : OcelotMainWindow
{
    public MainWindow(Plugin plugin, Config config)
        : base(plugin, config) { }

    public override void Draw()
    {
        if (!Helpers.IsInOccultCrescent())
        {
            ImGui.TextUnformatted("Not in Occult Crescent zone.");
            return;
        }

        plugin.modules?.DrawMainUi();
    }
}
