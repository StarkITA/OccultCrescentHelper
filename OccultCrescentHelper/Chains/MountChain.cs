using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using Ocelot.Chain;
using Ocelot.Chain.ChainEx;

namespace OccultCrescentHelper.Chains;

public class MountChain : ChainFactory
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
            .Wait(500)
            .Then(_ => ActionManager.Instance()->UseAction(ActionType.Mount, mountId));
    }
}
