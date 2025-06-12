using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using Ocelot.Chain;
using Ocelot.Chain.ChainEx;

namespace OccultCrescentHelper.Chains;

public class MountChain : RetryChainFactory
{
    private uint mountId;

    public MountChain(uint mountId = 1)
    {
        this.mountId = mountId;
    }

    protected unsafe override Chain Create(Chain chain)
    {
        return chain
            .BreakIf(() => Svc.Condition[ConditionFlag.Mounted])
            .WaitGcd()
            .WaitUntilNotCasting()
            .Then(_ => ActionManager.Instance()->UseAction(ActionType.Mount, mountId))
            .WaitUntilCasting()
            .WaitUntilNotCasting();
    }

    public override bool IsComplete() => Svc.Condition[ConditionFlag.Mounted];
}
