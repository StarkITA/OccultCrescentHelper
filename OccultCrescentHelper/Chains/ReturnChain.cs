using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using OccultCrescentHelper.Data;
using Ocelot.Chain;
using Ocelot.Chain.ChainEx;
using Ocelot.IPC;

namespace OccultCrescentHelper.Chains;

public class ReturnChain : RetryChainFactory
{
    private bool approachAetherye = false;

    private Vector3 destination = Vector3.Zero;

    private YesAlready? yes;

    private VNavmesh? vnav;

    private bool complete = false;

    public ReturnChain(Vector3 destination, YesAlready? yes = null, VNavmesh? vnav = null, bool approachAetherye = true)
    {
        this.approachAetherye = approachAetherye;
        this.destination = destination;
        this.yes = yes;
        this.vnav = vnav;
    }

    protected override Chain Create(Chain chain)
    {
        chain.BreakIf(() => Svc.ClientState.LocalPlayer?.IsDead == true);

        var zone = Svc.ClientState.TerritoryType;
        var costToReturn = 60f + Vector3.Distance(ZoneData.startingLocations[zone], destination);
        var costToWalk = Vector3.Distance(Player.Position, destination);

        if (costToReturn < costToWalk || vnav == null)
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
                    .Then(_ => vnav.MoveToPath([destination], false))
                    .WaitUntilNear(vnav, destination, 4f)
                    .Then(_ => vnav.Stop());
            }
        }
        else
        {
            chain
                .Then(new PathfindAndMoveToChain(vnav, destination))
                .WaitUntilNear(vnav, destination, 4f)
                .Then(_ => vnav.Stop());
        }


        return chain.Then(_ => complete = true);
    }

    public override bool IsComplete() => complete;

    public override int GetMaxAttempts() => 5;

    public override TaskManagerConfiguration? Config()
    {
        return new() { TimeLimitMS = 60000 };
    }
}
