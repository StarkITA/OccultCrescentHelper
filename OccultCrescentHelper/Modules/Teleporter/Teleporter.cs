using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using OccultCrescentHelper.Data;
using OccultCrescentHelper.Enums;
using Ocelot;
using Ocelot.Chain;
using Ocelot.IPC;

namespace OccultCrescentHelper.Modules.Teleporter;

public class Teleporter
{
    private readonly TeleporterModule module;


    public Teleporter(TeleporterModule module)
    {
        this.module = module;
    }

    private Chain GetTeleportChain(Lifestream lifestream, Aethernet aethernet)
        => ChainBuilder.Begin()
            .Log("Starting Teleport Chain")
            .Then(() => lifestream.AethernetTeleportByPlaceNameId!((uint)aethernet))
            .WaitUntil(() => Svc.Condition[ConditionFlag.BetweenAreas])
            .WaitWhile(() => Svc.Condition[ConditionFlag.BetweenAreas])
            .Build();

    private Chain GetMountChain()
        => ChainBuilder.Begin()
            .Log("Starting Mount Chain")
            .BreakIf(() => !module.config.ShouldMount)
            .Wait(500)
            .ThenOnFrameworkThread(() => Mount())
            .Build();

    private Chain GetFollowPathChain(VNavmesh vnav, Vector3 start, List<Vector3> path)
        => ChainBuilder.Begin()
            .Log("Starting Follow Path Chain")
            .BreakIf(() => !module.config.PathToDestination)
            .Then(() => vnav.PathfindAndMoveTo!(start, false))
            .Then(vnav.WaitToStart)
            .Then(vnav.WaitToStop)
            .Then(() => vnav.FollowPath!(path, false))
            .Build();

    private Chain GetPathfindAndMoveToChain(VNavmesh vnav, Vector3 destination)
        => ChainBuilder.Begin()
            .Log("Starting Pathfinding Chain")
            .BreakIf(() => !module.config.PathToDestination)
            .Then(() => vnav.PathfindAndMoveTo!(destination, false))
            .Build();

    private Chain GetPathfindingChain(VNavmesh vnav, EventData ev, Vector3 destination)
    {
        if (ev.customPath != null && ev.customPath.Count() > 0)
        {
            var pos = Svc.ClientState.LocalPlayer?.Position ?? Vector3.Zero;
            var path = ev.GetPath(pos, destination);
            var start = path[0];
            path.RemoveAt(0);

            return ChainBuilder.Begin()
                .Merge(GetMountChain())
                .Merge(GetFollowPathChain(vnav!, start, path))
                .Build();
        }

        return ChainBuilder.Begin()
            .Merge(GetMountChain())
            .Merge(GetPathfindAndMoveToChain(vnav, destination))
            .Build();
    }

    private unsafe void Mount()
    {
        if (!Svc.Condition[ConditionFlag.Mounted])
        {
            ActionManager.Instance()->UseAction(ActionType.Mount, module.config.Mount);
        }
    }

    public void Button(Aethernet? aethernet, Vector3 destination, string name, string id, EventData ev)
    {
        if (!module.TryGetIPCProvider<VNavmesh>(out var vnav) || vnav == null || !vnav.IsReady())
        {
            return;
        }

        if (!module.TryGetIPCProvider<Lifestream>(out var lifestream) || lifestream == null || !lifestream.IsReady())
        {
            return;
        }

        if (aethernet == null)
        {
            aethernet = GetClosestAethernet(destination);
        }

        var random = new Random();
        double angle = random.NextDouble() * Math.PI * 2;
        double radius = random.NextDouble() * 20f;
        float offsetX = (float)(Math.Cos(angle) * radius);
        float offsetZ = (float)(Math.Sin(angle) * radius);
        var point = new Vector3(destination.X + offsetX, destination.Y, destination.Z + offsetZ);

        var isNearShards = GetNearbyAethernetShards().Count() > 0;
        var isNearCurrentShard = IsNear((Aethernet)aethernet);

        OcelotUI.Indent(() => {
            if (ImGuiEx.IconButton(Dalamud.Interface.FontAwesomeIcon.Running, $"{name}##{id}"))
            {
                Svc.Log.Info($"Pathfinding to {name} at {destination}");
                ChainBuilder.Begin()
                    .Log("Starting Pathfinding Action sequence")
                    .Merge(GetMountChain())
                    .Merge(GetPathfindingChain(vnav, ev, destination))
                    .Run();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip($"Pathfind to {name}");
            }

            ImGui.SameLine();

            if (ImGuiEx.IconButton(Dalamud.Interface.FontAwesomeIcon.LocationArrow, $"{name}##{id}", enabled: isNearShards && !isNearCurrentShard))
            {
                ChainBuilder.Begin()
                    .Log("Starting Teleport Action sequence")
                    .Merge(GetTeleportChain(lifestream, (Aethernet)aethernet))
                    .Merge(GetMountChain())
                    .Merge(GetPathfindingChain(vnav, ev, destination))
                    .Run();
            }

            if (ImGui.IsItemHovered())
            {
                if (!isNearShards)
                {
                    ImGui.SetTooltip($"You must be near an aetheryte to teleport");
                }
                else if (isNearCurrentShard)
                {
                    ImGui.SetTooltip($"You're already at this aetheryte");
                }
                else
                {
                    ImGui.SetTooltip($"Teleport to {aethernet?.ToFriendlyString()}");
                }
            }
        });
    }

    private Aethernet GetClosestAethernet(Vector3 position)
        => AethernetData.All().OrderBy((data) => Vector3.Distance(position, data.position)).FirstOrDefault()!.aethernet;

    public IList<IGameObject> GetNearbyAethernetShards()
    {
        var playerPos = Svc.ClientState.LocalPlayer?.Position ?? Vector3.Zero;

        return Svc.Objects
                .Where(o => o.ObjectKind == ObjectKind.EventObj)
                .Where(o => AethernetData.All().Select((datum) => datum.dataId).Contains(o.DataId))
                .Where(o => Vector3.Distance(o.Position, playerPos) <= 4.5f)
                .ToList();
    }

    private bool IsNear(Aethernet aethernet) => GetNearbyAethernetShards().Where(o => o.DataId == aethernet.GetData().dataId).Count() > 0;

    public bool IsReady() => module.TryGetIPCProvider<Lifestream>(out var _);
}
