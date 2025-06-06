using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using OccultCrescentHelper.Windows;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.Debug;

#if DEBUG_BUILD
[OcelotModule]
#endif
public class DebugModule : Module<Plugin, Config>
{
    private Panel panel = new();

    private readonly WindowSystem windows = new($"OCH##Debug");

    public DebugWindow window { get; private set; }

    public DebugModule(Plugin plugin, Config config)
        : base(plugin, config) { }

    public override void Initialize()
    {
        window = new(plugin, plugin.config);
        windows.AddWindow(window);
        Svc.PluginInterface.UiBuilder.Draw += windows.Draw;
        window.Toggle();
    }

    public void DrawPanel() => panel.Draw(this);


    public override void OnTerritoryChanged(ushort id) => panel.OnTerritoryChanged(id);


    public override void Dispose()
    {
        Svc.PluginInterface.UiBuilder.Draw -= windows.Draw;
    }
}
