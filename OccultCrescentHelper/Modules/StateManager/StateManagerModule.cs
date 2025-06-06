
using System;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.StateManager;

[OcelotModule(mainOrder: -1)]
public class StateManagerModule : Module<Plugin, Config>
{
    public override StateManagerConfig config {
        get => _config.StateManagerConfig;
    }

    private Panel panel = new();

    private StateManager state = new();

    public event Action? OnEnterIdle {
        add => state.OnEnterIdle += value;
        remove => state.OnEnterIdle -= value;
    }

    public event Action? OnExitIdle {
        add => state.OnExitIdle += value;
        remove => state.OnExitIdle -= value;
    }

    public event Action? OnEnterInCombat {
        add => state.OnEnterInCombat += value;
        remove => state.OnEnterInCombat -= value;
    }

    public event Action? OnExitInCombat {
        add => state.OnExitInCombat += value;
        remove => state.OnExitInCombat -= value;
    }

    public event Action? OnEnterInFate {
        add => state.OnEnterInFate += value;
        remove => state.OnEnterInFate -= value;
    }

    public event Action? OnExitInFate {
        add => state.OnExitInFate += value;
        remove => state.OnExitInFate -= value;
    }

    public event Action? OnEnterInCriticalEngagement {
        add => state.OnEnterInCriticalEngagement += value;
        remove => state.OnEnterInCriticalEngagement -= value;
    }

    public event Action? OnExitInCriticalEngagement {
        add => state.OnExitInCriticalEngagement += value;
        remove => state.OnExitInCriticalEngagement -= value;
    }


    public StateManagerModule(Plugin plugin, Config config)
        : base(plugin, config) { }


    public override void Tick(IFramework framework) => state.Tick(framework);

    public override void OnChatMessage(XivChatType type, int timestamp, SeString sender, SeString message, bool isHandled)
        => state.OnChatMessage(type, timestamp, sender, message, isHandled);

    public override bool DrawMainUi() => panel.Draw(this);

    public string GetStateText() => state.GetState().ToString();
}
