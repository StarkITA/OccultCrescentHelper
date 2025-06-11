using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Game.ClientState.Objects.Enums;
using ECommons.Automation;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using OccultCrescentHelper.Chains;
using OccultCrescentHelper.Data;
using OccultCrescentHelper.Enums;
using OccultCrescentHelper.Modules.CriticalEncounters;
using OccultCrescentHelper.Modules.StateManager;
using Ocelot.Chain;
using Ocelot.Chain.ChainEx;
using Ocelot.IPC;

namespace OccultCrescentHelper.Modules.Automator;


public class Activity
{
    private const float TARGET_DISTANCE = 20f;

    public readonly EventData data;

    private Lifestream lifestream;

    private VNavmesh vnav;

    public Func<bool> isValid = () => true;

    public Func<Vector3> getPosition = () => Vector3.Zero;

    public ActivityState state = ActivityState.Idle;

    private uint mount = 1;

    private bool delay = false;

    private bool bmrToggle = true;

    private Dictionary<ActivityState, Func<StateManagerModule, Func<Chain>?>> handlers;

    private Activity(EventData data, Lifestream lifestream, VNavmesh vnav)
    {
        this.data = data;
        this.lifestream = lifestream;
        this.vnav = vnav;

        handlers = new() {
            { ActivityState.Idle, GetIdleChain },
            { ActivityState.Pathfinding, GetPathfindingChain },
            { ActivityState.WaitingToStartCriticalEncoutner, GetWaitingToStartCriticalEncoutnerChain },
            { ActivityState.Participating, GetParticipatingChain },
            { ActivityState.Done, GetDoneChain },
        };
    }

    public static Activity ForCriticalEncounter(DynamicEvent encounter, EventData data, Lifestream lifestream, VNavmesh vnav, CriticalEncountersModule critical)
    {
        return new(data, lifestream, vnav) {
            isValid = () => critical.criticalEncounters[(int)data.id].State != DynamicEventState.Inactive,
            getPosition = () => encounter.MapMarker.Position,
        };
    }

    public static Activity ForFate(IFate fate, EventData data, Lifestream lifestream, VNavmesh vnav)
    {
        return new(data, lifestream, vnav) {
            isValid = () => Svc.Fates.Contains(fate),
            getPosition = () => data.start ?? fate.Position,
        };
    }

    public Activity WithMountId(uint mount)
    {
        this.mount = mount;
        return this;
    }

    public Activity WithDelay(bool delay)
    {
        this.delay = delay;
        return this;
    }

    public Activity WithBmrToggle(bool bmrToggle)
    {
        this.bmrToggle = bmrToggle;
        return this;
    }

    public unsafe Func<Chain>? GetChain(StateManagerModule states)
    {
        if (!isValid())
        {
            return () => Chain.Create("Dummy Chain");
        }

        return handlers[state](states);
    }

    public Func<Chain>? GetIdleChain(StateManagerModule states)
    {
        return () => {
            return Chain.Create("Illegal:Idle")
                .ConditionalThen(_ => bmrToggle, _ => Chat.ExecuteCommand("/bmrai off"))
                .Then(_ => vnav.Stop())
                .Then(_ => state = ActivityState.Pathfinding);
        };
    }

    public Func<Chain>? GetPathfindingChain(StateManagerModule states)
    {
        return () => {
            var playerShard = AethernetData.All().OrderBy((data) => Vector3.Distance(Player.Position, data.position)).First();
            var activityShard = GetAethernetData();

            bool isFate = data.type == EventType.Fate;

            return Chain.Create("Illegal:Pathfinding")
                .ConditionalWait(_ => delay, Random.Shared.Next(10000, 15001))
                .ConditionalThen(_ => playerShard.dataId != activityShard.dataId, new TeleportChain(lifestream, activityShard.aethernet))
                .Then(new MountChain(mount))
                .Then(new PathfindingChain(vnav, getPosition(), data, false, 20f, 10f))
                .WaitToStartPathfinding(vnav)
                // Fate
                .ConditionalThen(_ => isFate, GetFatePathfindingWatcher(states, vnav))
                .ConditionalThen(_ => isFate, _ => state = ActivityState.Participating)
                // Critical Encounter
                .ConditionalThen(_ => !isFate, GetCriticalEncounterPathfindingWatcher(states, vnav))
                .ConditionalThen(_ => !isFate, _ => state = ActivityState.WaitingToStartCriticalEncoutner);
        };
    }

    public Func<Chain>? GetWaitingToStartCriticalEncoutnerChain(StateManagerModule states)
    {
        return () => {
            return Chain.Create("Illegal:WaitingToStartCriticalEncoutner")
                .Then(new TaskManagerTask(() => {
                    // @todo check we don't leave the area
                    return states.GetState() == State.InCriticalEncounter;
                }, new() { TimeLimitMS = 180000 }))
                .Then(_ => state = ActivityState.Participating);
        };
    }

    public Func<Chain>? GetParticipatingChain(StateManagerModule states)
    {
        return () => {
            return Chain.Create("Illegal:Participating")
                .ConditionalThen(_ => bmrToggle, _ => Chat.ExecuteCommand("/bmrai on"))
                    .Then(_ => vnav.Stop())
                    .Then(new TaskManagerTask(() => {
                        if (EzThrottler.Throttle("Participating.ForceTarget", 100))
                        {
                            Svc.Targets.Target ??= Svc.Objects
                                .Where(o =>
                                    o != null &&
                                    o.ObjectKind == ObjectKind.BattleNpc &&
                                    o.IsHostile() &&
                                    o.IsTargetable
                                )
                                .OrderBy(o => Vector3.Distance(o.Position, Player.Position))
                                .ToList()
                                .FirstOrDefault();
                        }

                        return states.GetState() == State.Idle;
                    }, new() { TimeLimitMS = int.MaxValue }))
                    .Then(_ => state = ActivityState.Done);
        };
    }

    public Func<Chain>? GetDoneChain(StateManagerModule states)
    {
        return null;
    }

    private unsafe TaskManagerTask GetFatePathfindingWatcher(StateManagerModule states, VNavmesh vnav)
    {
        bool runningToTarget = false;
        return new(() => {
            if (EzThrottler.Throttle("FatePathfindingWatcher.EnemyScan", 100))
            {
                Svc.Targets.Target ??= Svc.Objects
                    .Where(o =>
                        o != null &&
                        o.DataId == (uint)data.monster! &&
                        o.IsTargetable
                    )
                    .OrderBy(o => Vector3.Distance(o.Position, Player.Position))
                    .ToList()
                    .FirstOrDefault();
            }

            if (Svc.Targets.Target != null)
            {
                if (!runningToTarget)
                {
                    vnav.PathfindAndMoveTo(Svc.Targets.Target.Position, false);
                    runningToTarget = true;
                }

                if (states.GetState() == State.InFate)
                {
                    if (Vector3.Distance(Player.Position, Svc.Targets.Target.Position) <= 5f)
                    {
                        // Dismount
                        if (Svc.Condition[ConditionFlag.Mounted])
                        {
                            ActionManager.Instance()->UseAction(ActionType.Mount, 1);
                        }

                        vnav.Stop();

                        return true;
                    }
                }
            }

            if (!vnav.IsRunning())
            {
                throw new VnavmeshStoppedException();
            }

            return false;
        }, new() { TimeLimitMS = 180000, ShowError = false });
    }

    private TaskManagerTask GetCriticalEncounterPathfindingWatcher(StateManagerModule states, VNavmesh vnav)
    {
        return new(() => {
            if (!vnav.IsRunning())
            {
                throw new VnavmeshStoppedException();
            }

            return vnav.IsRunning() || IsInZone();
        }, new() { TimeLimitMS = 180000, ShowError = false });
    }

    public AethernetData GetAethernetData()
        => data.aethernet?.GetData() ?? AethernetData.All().OrderBy((data) => Vector3.Distance(getPosition(), data.position)).First();

    public bool IsInZone()
    {
        return Vector3.Distance(Player.Position, getPosition()) <= 20f;
    }
}
