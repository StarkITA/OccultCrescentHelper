
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using OccultCrescentHelper.Modules.Mount.Chains;
using OccultCrescentHelper.Modules.StateManager;
using Ocelot.Chain;
using Ocelot.IPC;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.Mount;

[OcelotModule(configOrder: int.MinValue)]
public class MountModule : Module<Plugin, Config>
{
    public static ChainQueue MountMaintainer => ChainManager.Get("OCH##MountMaintainer");

    public override MountConfig config {
        get => _config.MountConfig;
    }

    public MountModule(Plugin plugin, Config config)
        : base(plugin, config) { }

    public override void Tick(IFramework _)
    {
        if (_config.AutomatorConfig.Enabled)
        {
            return;
        }

        if (config.MaintainMount)
        {
            MaintainMount();
        }
    }

    public void MaintainMount()
    {
        if (Svc.Condition[ConditionFlag.Mounted])
        {
            return;
        }

        if (TryGetIPCProvider<Lifestream>(out var lifestream) && lifestream != null && lifestream.IsBusy())
        {
            return;
        }

        if (!TryGetModule<StateManagerModule>(out var states) || states == null || states.GetState() != State.Idle)
        {
            return;
        }

        var player = Svc.ClientState.LocalPlayer;
        if (player == null || player.IsCasting)
        {
            return;
        }

        if (!MountMaintainer.IsRunning)
        {
            MountMaintainer.Submit(new MountChain(config));
        }
    }
}
