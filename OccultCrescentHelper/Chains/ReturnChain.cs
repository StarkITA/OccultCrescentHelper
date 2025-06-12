using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using OccultCrescentHelper.Enums;
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

    public ReturnChain(Vector3 destination, YesAlready? yes = null, VNavmesh? vnav = null, bool approachAetherye = true)
    {
        this.approachAetherye = approachAetherye;
        this.destination = destination;
        this.yes = yes;
        this.vnav = vnav;
    }

    protected override Chain Create(Chain chain)
    {
        AethernetData closest = AethernetData.GetClosestToPlayer();
        float distance = closest.Distance();
        if (distance <= 60f && vnav != null)
        {
            if (distance > 4.2f)
            {
                return chain
                    .Then(new PathfindAndMoveToChain(vnav, closest.position, 4f, 3f))
                    .WaitUntilNear(vnav, closest.position);
            }

            return chain;
        }

        yes?.PausePlugin(5000);

        chain
            .BreakIf(() => Svc.ClientState.LocalPlayer?.IsDead == true)
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
    public override TaskManagerConfiguration? Config()
    {
        return new() { TimeLimitMS = 60000 };
    }
}
