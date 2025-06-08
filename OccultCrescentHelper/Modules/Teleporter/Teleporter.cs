using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using OccultCrescentHelper.Data;
using OccultCrescentHelper.Enums;
using Ocelot;
using Ocelot.Chain;
using Ocelot.Chain.ChainEx;
using Ocelot.IPC;

namespace OccultCrescentHelper.Modules.Teleporter;

public class Teleporter
{
    private readonly TeleporterModule module;

    private readonly TaskManager taskManager = new();

    public Teleporter(TeleporterModule module)
    {
        this.module = module;
    }

    private readonly Dictionary<uint, Vector3> aetherytes = new() { { 1252, new Vector3(830.75f, 72.98f, -695.98f) } };

    private Chain GetTeleportChain(Lifestream lifestream, Aethernet aethernet)
        => Chain.Create("Teleport")
            .Then(_ => lifestream.AethernetTeleportByPlaceNameId((uint)aethernet))
            .WaitToCycleCondition(ConditionFlag.BetweenAreas);

    private Chain GetMountChain()
        => Chain.Create("Mount")
            .BreakIf(() => !module.config.ShouldMount)
            .Wait(500)
            .Then(_ => Mount());

    private Chain GetFollowPathChain(VNavmesh vnav, Vector3 start, List<Vector3> path)
        => Chain.Create("Follow Path")
            .BreakIf(() => !module.config.PathToDestination)
            .PathfindAndMoveTo(vnav, start)
            .WaitUntilNear(vnav, start)
            .Log($"Near {start}")
            .Then(_ => vnav.FollowPath(path, false));

    private Chain GetPathfindAndMoveToChain(VNavmesh vnav, Vector3 destination)
        => Chain.Create("Pathfinding")
            .BreakIf(() => !module.config.PathToDestination)
            .PathfindAndMoveTo(vnav, destination);

    private Chain GetPathfindingChain(VNavmesh vnav, EventData ev, Vector3 destination)
    {
        if (ev.pathFactory != null)
        {
            return Chain.Create("Mounth & Follow")
                .Then(() => GetMountChain())
                .Then(Prowler.Create(new(vnav), ev.pathFactory()()));
        }

        return Chain.Create()
            .Then(() => GetMountChain())
            .Then(() => GetPathfindAndMoveToChain(vnav, destination));
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

        if (aethernet == null)
        {
            aethernet = GetClosestAethernet(destination);
        }

        OcelotUI.Indent(() => {
            PathfindingButton(destination, name, id, ev);
            TeleportButton((Aethernet)aethernet, destination, name, id, ev);
        });
    }

    private void PathfindingButton(Vector3 destination, string name, string id, EventData ev)
    {
        if (!module.TryGetIPCProvider<VNavmesh>(out var vnav) || vnav == null || !vnav.IsReady())
        {
            return;
        }

        if (ImGuiEx.IconButton(Dalamud.Interface.FontAwesomeIcon.Running, $"{name}##{id}"))
        {
            Svc.Log.Info($"Pathfinding to {name} at {destination}");

            Chain.Create("Mount & Pathfinding")
                .Then(() => GetMountChain())
                .Then(() => GetPathfindingChain(vnav, ev, destination));
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip($"Pathfind to {name}");
        }


        if (!module.TryGetIPCProvider<Lifestream>(out var lifestream) || lifestream == null || !lifestream.IsReady())
        {
            return;
        }

        ImGui.SameLine();
    }

    private void TeleportButton(Aethernet aethernet, Vector3 destination, string name, string id, EventData ev)
    {
        if (!module.TryGetIPCProvider<Lifestream>(out var lifestream) || lifestream == null || !lifestream.IsReady())
        {
            return;
        }

        var isNearShards = GetNearbyAethernetShards().Count() > 0;
        var isNearCurrentShard = IsNear((Aethernet)aethernet);

        if (ImGuiEx.IconButton(Dalamud.Interface.FontAwesomeIcon.LocationArrow, $"{name}##{id}", enabled: isNearShards && !isNearCurrentShard))
        {
            var chain = Chain.Create("Teleport Sequence")
                .Then(() => GetTeleportChain(lifestream, aethernet))
                .Then(() => GetMountChain());

            if (module.TryGetIPCProvider<VNavmesh>(out var vnav) && vnav != null && vnav.IsReady())
            {
                var random = new Random();
                double angle = random.NextDouble() * Math.PI * 2;
                double radius = random.NextDouble() * 20f;
                float offsetX = (float)(Math.Cos(angle) * radius);
                float offsetZ = (float)(Math.Sin(angle) * radius);
                var point = new Vector3(destination.X + offsetX, destination.Y, destination.Z + offsetZ);

                chain.Then(() => GetPathfindingChain(vnav, ev, point));
            }
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
                ImGui.SetTooltip($"Teleport to {aethernet.ToFriendlyString()}");
            }
        }
    }

    public void OnFateEnd()
    {
        if (!module.config.ReturnAfterFate)
        {
            return;
        }

        Return();
    }

    public void OnCriticalEncounterEnd()
    {
        if (!module.config.ReturnAfterCritcalEncounter)
        {
            return;
        }

        Return();
    }

    public void Return()
    {
        if (Helpers.IsInForkedTower())
        {
            return;
        }

        var player = Svc.ClientState.LocalPlayer;
        if (player == null)
        {
            return;
        }

        var chain = Chain.Create()
            .UseGcdAction(ActionType.GeneralAction, 8)
            .AddonCallback("SelectYesno", true, 0)
            .WaitToCast()
            .WaitToCycleCondition(ConditionFlag.BetweenAreas);

        if (module.config.ApproachAetheryte && module.TryGetIPCProvider<VNavmesh>(out var vnav) && vnav != null && vnav.IsReady())
        {
            Random random = new();
            float angle = (float)(random.NextDouble() * Math.PI * 2);
            float distance = (float)(Math.Sqrt(random.NextDouble()) * (4f - 3f) + 3f);

            float xOffset = (float)Math.Cos(angle) * distance;
            float zOffset = (float)Math.Sin(angle) * distance;

            var aetheryte = aetherytes[Svc.ClientState.TerritoryType];
            var destination = new Vector3(aetheryte.X + xOffset, aetheryte.Y, aetheryte.Z + zOffset);

            chain.Wait(500);
            chain.PathfindAndMoveTo(vnav, destination);
        }
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
