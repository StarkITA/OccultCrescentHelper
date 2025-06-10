using System;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using ECommons.Automation.LegacyTaskManager;
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
using OccultCrescentHelper.Modules.Debug.Panels;
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

    private Activity(EventData data, Lifestream lifestream, VNavmesh vnav)
    {
        this.data = data;
        this.lifestream = lifestream;
        this.vnav = vnav;
    }

    public static Activity ForCriticalEncounter(DynamicEvent encounter, EventData data, Lifestream lifestream, VNavmesh vnav)
    {
        return new(data, lifestream, vnav) {
            isValid = () => encounter.State != DynamicEventState.Inactive,
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

    public unsafe Func<Chain> GetChain(StateManagerModule states)
    {
        return () => {
            return Chain.Create($"Auto Activity [{data.Name}]")
                .Then(_ => vnav.Stop())
                .Then(_ => state = ActivityState.Pathfinding)
                .Then(() => {
                    var chain = Chain.Create("Pathfinding");

                    if (delay)
                    {
                        chain.Wait(Random.Shared.Next(10000, 15001));
                    }

                    var playerShard = AethernetData.All().OrderBy((data) => Vector3.Distance(Player.Position, data.position)).First();
                    var activityShard = GetAethernetData();
                    if (playerShard.dataId != activityShard.dataId)
                    {
                        chain.Then(new TeleportChain(lifestream, activityShard.aethernet));
                    }

                    return chain
                        .Then(new MountChain(mount))
                        .Then(new PathfindingChain(vnav, getPosition(), data, false, 20f, 10f));
                })
                .Then(() => {
                    var chain = Chain.Create("Waiting To Start");

                    if (data.type == EventType.Fate)
                    {
                        chain
                            .Then(_ => state = ActivityState.WaitingForFateTarget)
                            .Then(GetWaitingForFateTargetTask(states))
                            .Then(_ => {
                                if (Svc.Condition[ConditionFlag.Mounted])
                                {
                                    ActionManager.Instance()->UseAction(ActionType.Mount, 1);
                                }

                                vnav.Stop();
                            });
                    }
                    else if (data.type == EventType.CriticalEncounter)
                    {
                        chain
                            .Then(_ => state = ActivityState.WaitingToStartCriticalEncoutner)
                            .Then(new TaskManagerTask(() => { return states.GetState() == State.InCriticalEncounter; }, new() { TimeLimitMS = 180000 }))
                            .Then(_ => state = ActivityState.Participating);
                    }

                    return chain;
                })
                .Then(_ => state = ActivityState.Participating)
                .Then(states.GetState() == State.InFate ? GetForceTargetInFateTask(states) : new TaskManagerTask(() => true))
                .Then(new TaskManagerTask(() => {
                    if (states.GetState() == State.InCriticalEncounter)
                    {
                        var enemy = Svc.Objects
                            .Where(o =>
                                o != null &&
                                o.IsTargetable &&
                                Vector3.Distance(o.Position, Player.Position) <= TARGET_DISTANCE
                            )
                            .OrderBy(o => Vector3.Distance(o.Position, Player.Position))
                            .ToList()
                            .FirstOrDefault();
                    }

                    return states.GetState() == State.Idle;
                }, new() { TimeLimitMS = int.MaxValue }))
                .Then(_ => state = ActivityState.Idle)
                .Wait(5000)
                .Log($"Done {data.Name}");
        };
    }

    private TaskManagerTask GetWaitingForFateTargetTask(StateManagerModule states)
    {
        return new TaskManagerTask(() => {
            if (EzThrottler.Throttle("FateWaitingToStart##enemies", 50))
            {
                var enemy = Svc.Objects
                    .Where(o =>
                        o != null &&
                        o.DataId == (uint)data.monster! &&
                        o.IsTargetable &&
                        Vector3.Distance(o.Position, Player.Position) <= TARGET_DISTANCE
                    )
                    .OrderBy(o => Vector3.Distance(o.Position, Player.Position))
                    .ToList()
                    .FirstOrDefault();

                if (enemy != null)
                {
                    Svc.Log.Info("Targeting");
                    Svc.Targets.Target = enemy;

                    return Svc.Targets.Target != null;
                }
            }

            return states.GetState() == State.InFate || Svc.Targets.Target != null;
        });
    }

    private TaskManagerTask GetForceTargetInFateTask(StateManagerModule states)
    {
        return new TaskManagerTask(() => {
            if (EzThrottler.Throttle("FateWaitingToStart##enemies", 50) && Svc.Targets.Target != null)
            {
                var enemy = Svc.Objects
                    .Where(o =>
                        o != null &&
                        o.DataId == (uint)data.monster! &&
                        o.IsTargetable &&
                        Vector3.Distance(o.Position, Player.Position) <= TARGET_DISTANCE
                    )
                    .OrderBy(o => Vector3.Distance(o.Position, Player.Position))
                    .ToList()
                    .FirstOrDefault();

                if (enemy != null)
                {
                    Svc.Log.Info("Targeting");
                    Svc.Targets.Target = enemy;
                }
            }


            return states.GetState() == State.Idle;
        });
    }

    public AethernetData GetAethernetData()
        => data.aethernet?.GetData() ?? AethernetData.All().OrderBy((data) => Vector3.Distance(getPosition(), data.position)).First();

    public bool IsInZone()
    {
        return Vector3.Distance(Player.Position, getPosition()) <= 20f;
    }
}
