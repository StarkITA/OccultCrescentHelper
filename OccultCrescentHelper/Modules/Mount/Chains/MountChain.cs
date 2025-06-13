using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using Ocelot.Chain;
using Ocelot.Chain.ChainEx;

namespace OccultCrescentHelper.Modules.Mount.Chains;

public class MountChain : RetryChainFactory
{
    private MountConfig config;

    public MountChain(MountConfig config)
    {
        this.config = config;
    }

    protected unsafe override Chain Create(Chain chain)
    {
        return chain
            .BreakIf(() => Svc.Condition[ConditionFlag.Mounted] || Svc.ClientState.LocalPlayer?.IsCasting == true)
            .ConditionalThen(_ => !config.MountRoulette, _ => ActionManager.Instance()->UseAction(ActionType.Mount, config.Mount))
            // Mount Roulette
            .ConditionalThen(_ => config.MountRoulette, _ => ActionManager.Instance()->UseAction(ActionType.GeneralAction, 9));
    }

    public override bool IsComplete() => Svc.Condition[ConditionFlag.Mounted];
}
