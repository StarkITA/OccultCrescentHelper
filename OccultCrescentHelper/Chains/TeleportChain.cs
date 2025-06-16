using Dalamud.Game.ClientState.Conditions;
using ECommons.Automation.LegacyTaskManager;
using ECommons.Automation.NeoTaskManager;
using OccultCrescentHelper.Enums;
using OccultCrescentHelper.Modules.Teleporter;
using Ocelot.Chain;
using Ocelot.Chain.ChainEx;
using Ocelot.IPC;

namespace OccultCrescentHelper.Chains;

public class TeleportChain : ChainFactory
{
    private Aethernet aethernet;

    private Lifestream lifestream;

    private TeleporterModule module;

    public TeleportChain(Aethernet aethernet, Lifestream lifestream, TeleporterModule module)
    {
        this.lifestream = lifestream;
        this.aethernet = aethernet;
        this.module = module;
    }

    protected override Chain Create(Chain chain)
    {
        return chain
            .Then(_ => lifestream.Abort())
            .Then(_ => module.GetIPCProvider<VNavmesh>()?.Stop())
            .Then(_ => lifestream.AethernetTeleportByPlaceNameId((uint)aethernet))
            .WaitToCycleCondition(ConditionFlag.BetweenAreas)
            .Debug("Waiting for lifestream to not be 'busy'")
            .Then(new TaskManagerTask(() => !lifestream.IsBusy(), new() { TimeLimitMS = 30000 }))
            .ConditionalThen(_ => module.config.ShouldMount, ChainHelper.MountChain());
    }
}
