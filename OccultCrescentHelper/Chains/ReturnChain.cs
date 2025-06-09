using System.Collections.Generic;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using FFXIVClientStructs.FFXIV.Client.Game;
using Ocelot.Chain;
using Ocelot.Chain.ChainEx;
using Ocelot.IPC;

namespace OccultCrescentHelper.Chains;

public class ReturnChain : ChainFactory
{
    private bool approachAetherye = false;

    private Vector3 destination = Vector3.Zero;

    private YesAlready? yes;

    private VNavmesh? vnav;

    public ReturnChain(YesAlready? yes = null, VNavmesh? vnav = null)
    {
        this.yes = yes;
        this.vnav = vnav;
    }

    public ReturnChain(Vector3 destination, YesAlready? yes = null, VNavmesh? vnav = null)
    {
        this.approachAetherye = true;
        this.destination = destination;
        this.yes = yes;
        this.vnav = vnav;
    }

    protected override Chain Create(Chain chain)
    {
        yes?.PausePlugin(5000);

        chain
            .UseGcdAction(ActionType.GeneralAction, 8)
            .AddonCallback("SelectYesno", true, 0)
            .WaitToCast()
            .WaitToCycleCondition(ConditionFlag.BetweenAreas);

        if (approachAetherye && vnav != null)
        {
            chain
                .Wait(500)
                .Then(new PathfindAndMoveToChain(vnav, destination, 4f, 3f))
                .WaitUntilNear(vnav, destination);
        }

        return chain;
    }
}
