using Dalamud.Game.Command;
using Dalamud.Plugin;
using ECommons;
using ECommons.DalamudServices;
using OccultCrescentHelper.Managers;

namespace OccultCrescentHelper;

public sealed class Plugin : IDalamudPlugin
{
    internal string Name = "Occult Crescent Helper";

    private const string Command = "/och";

    public Config config { get; init; }

    private readonly WindowManager windows;

    public readonly TrackersManager trackers;

    public readonly Overlay.Overlay overlay;

    public Plugin(IDalamudPluginInterface plugin)
    {
        ECommonsMain.Init(plugin, this);
        config = plugin.GetPluginConfig() as Config ?? new Config();

        windows = new WindowManager(this);
        Svc.Commands.AddHandler(
            Command,
            new CommandInfo((string command, string args) => windows.ToggleMainUI())
            {
                HelpMessage = $"Opens the {Name} window.",
            }
        );

        Svc.Framework.Update += TreasureManager.UpdateTreasureList;
        Svc.Framework.Update += FatesManager.UpdateFatesList;

        trackers = TrackersManager.Instance;
        Svc.Framework.Update += trackers.Tick;

        overlay = new Overlay.Overlay();

        Svc.PluginInterface.UiBuilder.Draw += overlay.Draw;
    }

    public void Dispose()
    {
        Svc.Framework.Update -= TreasureManager.UpdateTreasureList;
        Svc.Framework.Update -= FatesManager.UpdateFatesList;

        Svc.Commands.RemoveHandler(Command);
        Svc.Framework.Update -= trackers.Tick;

        windows.Dispose();

        Svc.PluginInterface.UiBuilder.Draw -= overlay.Draw;

        ECommonsMain.Dispose();
    }
}
