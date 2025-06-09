using Dalamud.Game.ClientState.Conditions;
using OccultCrescentHelper.Enums;
using Ocelot.Chain;
using Ocelot.Chain.ChainEx;
using Ocelot.IPC;

namespace OccultCrescentHelper.Chains;

public class TeleportChain : ChainFactory
{
    private Lifestream lifestream;

    private Aethernet aethernet;

    public TeleportChain(Lifestream lifestream, Aethernet aethernet)
    {
        this.lifestream = lifestream;
        this.aethernet = aethernet;
    }

    protected override Chain Create(Chain chain)
    {
        return chain
            .Then(_ => lifestream.AethernetTeleportByPlaceNameId((uint)aethernet))
            .WaitToCycleCondition(ConditionFlag.BetweenAreas);
    }
}
