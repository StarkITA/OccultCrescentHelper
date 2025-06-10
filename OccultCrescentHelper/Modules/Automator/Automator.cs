using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine;
using Lumina.Excel.Sheets;
using OccultCrescentHelper.Chains;
using OccultCrescentHelper.Data;
using OccultCrescentHelper.Enums;
using OccultCrescentHelper.Modules.CriticalEncounters;
using OccultCrescentHelper.Modules.Fates;
using OccultCrescentHelper.Modules.StateManager;
using OccultCrescentHelper.Modules.Teleporter;
using Ocelot.Chain;
using Ocelot.Chain.ChainEx;
using Ocelot.IPC;

namespace OccultCrescentHelper.Modules.Automator;

public class Automator
{
    private const float RETURN_RANGE = 60f;

    private const float AETHERNET_RANGE = 4f;

    private bool IsChainActive => ChainManager.Active().Count > 0;

    private readonly Dictionary<uint, Vector3> aetherytes = new() { { 1252, new Vector3(830.75f, 72.98f, -695.98f) } };

    public Activity? activity { get; private set; } = null;


    public void Tick(AutomatorModule module)
    {
        if (!module.TryGetIPCProvider<VNavmesh>(out var vnav) || vnav == null)
        {
            return;
        }

        if (!module.TryGetIPCProvider<Lifestream>(out var lifestream) || lifestream == null)
        {
            return;
        }

        if (!module.TryGetModule<StateManagerModule>(out var states) || states == null)
        {
            return;
        }

        if (states.GetState() != State.Idle)
        {
            return;
        }

        if (activity != null && !activity.isValid())
        {
            Plugin.Chain.Abort();
            vnav.Stop();
            activity = null;
        }

        if (IsChainActive)
        {
            return;
        }

        if (activity != null)
        {
            if (activity.state == ActivityState.WaitingToStartCriticalEncoutner && activity.IsInZone())
            {
                return;
            }


            Svc.Log.Info($"Trying to recover activity [{activity.data.Name}]");

            var playerDistance = Vector3.Distance(Player.Position, activity.getPosition());

            var aethernet = activity.GetAethernetData();
            var aetheryteDistance = Vector3.Distance(aethernet.position, activity.getPosition());

            if (playerDistance <= aetheryteDistance)
            {
                Plugin.Chain.Submit(new PathfindAndMoveToChain(vnav, activity.getPosition()));
                return;
            }

            activity = null;

            return;
        }

        Aethernet shard = AethernetData.All().OrderBy((data) => Vector3.Distance(Player.Position, data.position)).First().aethernet;
        var shardData = shard.GetData();
        var distance = Vector3.Distance(shardData.position, Player.Position);
        if (distance >= RETURN_RANGE)
        {
            if (!aetherytes.TryGetValue(Svc.ClientState.TerritoryType, out var returnPoint))
            {
                module.Warning($"No return point defined for territory: {Svc.ClientState.TerritoryType}");
                return;
            }

            Plugin.Chain.Submit(new ReturnChain(returnPoint, module.GetIPCProvider<YesAlready>(), vnav));
            return;
        }

        if (distance > AETHERNET_RANGE)
        {
            Plugin.Chain.Submit(() => {
                return Chain.Create("Moving to Aetheryte/Shard")
                    .Then(new PathfindAndMoveToChain(vnav, shard.GetData().position, 4f, 3f))
                    .WaitUntilNear(vnav, shard.GetData().position);
            });

            return;
        }

        if (!module.config.ShouldDoFates && !module.config.ShouldDoCriticalEncounters)
        {
            return;
        }

        // Try and get the next activity
        activity ??= module.config.ShouldDoCriticalEncounters ? FindCriticalEncounter(module, lifestream, vnav) : null;
        activity ??= module.config.ShouldDoFates ? FindFate(module, lifestream, vnav) : null;
        if (activity == null)
        {
            return;
        }

        Svc.Log.Info($"Selected activity: {activity.data.Name}");
        Plugin.Chain.Submit(activity.GetChain(states));
    }

    public Activity? FindCriticalEncounter(AutomatorModule module, Lifestream lifestream, VNavmesh vnav)
    {
        if (!module.TryGetModule<CriticalEncountersModule>(out var source) || source == null)
        {
            return null;
        }

        uint index = 0;
        foreach (var encounter in source.criticalEncounters)
        {
            if (
                !module.config.CriticalEncountersMap.TryGetValue(index, out var enabled) || !enabled
                || encounter.State != DynamicEventState.Register
                || !EventData.CriticalEncounters.TryGetValue(index, out var data)
            )
            {
                index++;
                continue;
            }

            return Activity.ForCriticalEncounter(encounter, data, lifestream, vnav).WithMountId(module.plugin.config.TeleporterConfig.Mount);
        }

        return null;
    }

    public Activity? FindFate(AutomatorModule module, Lifestream lifestream, VNavmesh vnav)
    {
        if (!module.TryGetModule<FatesModule>(out var source) || source == null)
        {
            return null;
        }

        foreach (var fate in source.fates.Values)
        {
            if (
                fate == null
                || !module.config.FatesMap[fate.FateId] == true
                || !EventData.Fates.TryGetValue(fate.FateId, out var data)
            )
            {
                continue;
            }

            return Activity.ForFate(fate, data, lifestream, vnav).WithMountId(module.plugin.config.TeleporterConfig.Mount);
        }

        return null;
    }

}
