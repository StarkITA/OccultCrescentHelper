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

    private const string ConfigCommand = "/ochc";

    public Config config { get; init; }

    private readonly WindowManager windows;

    public readonly TrackersManager trackers;

    public readonly JobManager jobs;

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

        Svc.Commands.AddHandler(
            ConfigCommand,
            new CommandInfo((string command, string args) => windows.ToggleConfigUI())
            {
                HelpMessage = $"Opens the {Name} config window.",
            }
        );

        Svc.Framework.Update += TreasureManager.UpdateTreasureList;
        Svc.Framework.Update += FatesManager.UpdateFatesList;
        Svc.Framework.Update += CarrotManager.UpdateCarrotList;

        trackers = TrackersManager.Instance;
        Svc.Framework.Update += trackers.Tick;

        jobs = new JobManager(this);
        Svc.Framework.Update += jobs.Tick;
        Svc.Chat.ChatMessage += jobs.OnChatMessage;

        overlay = new Overlay.Overlay(this);
        Svc.PluginInterface.UiBuilder.Draw += overlay.Draw;

        Svc.Log.Info(Svc.ClientState.TerritoryType.ToString());
    }

    public void Dispose()
    {
        Svc.Framework.Update -= TreasureManager.UpdateTreasureList;
        Svc.Framework.Update -= FatesManager.UpdateFatesList;
        Svc.Framework.Update -= CarrotManager.UpdateCarrotList;

        Svc.Commands.RemoveHandler(Command);
        Svc.Framework.Update -= trackers.Tick;

        Svc.Framework.Update -= jobs.Tick;
        Svc.Chat.ChatMessage -= jobs.OnChatMessage;

        windows.Dispose();

        Svc.PluginInterface.UiBuilder.Draw -= overlay.Draw;

        ECommonsMain.Dispose();
    }
}
