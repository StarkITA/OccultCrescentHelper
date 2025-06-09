using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using OccultCrescentHelper.Chains;
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

    private Chain GetMountChain()
        => Chain.Create()
            .RunIf(() => module.config.ShouldMount)
            .Then(new MountChain(module.config.Mount));

    private Chain GetPathfindingChain(VNavmesh vnav, EventData ev, Vector3 destination, float radius = 16f)
    {
        if (module.config.UseCustomPaths && ev.pathFactory != null)
        {
            module.Debug("Using custom path");
            return Chain.Create("Prowler")
                .Then(new ProwlerChain(vnav, ev.pathFactory, destination));
        }

        return Chain.Create("Pathfinding")
            .Then(new PathfindAndMoveToChain(vnav, destination, radius));
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

            ChainManager.Submit(
                () => Chain.Create("Mount & Pathfinding")
                    .Then(GetMountChain)
                    .Then(() => GetPathfindingChain(vnav, ev, destination, 20f))
            );
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
        var isNearCurrentShard = IsNear(aethernet);

        if (ImGuiEx.IconButton(Dalamud.Interface.FontAwesomeIcon.LocationArrow, $"{name}##{id}", enabled: isNearShards && !isNearCurrentShard))
        {

            var factory = () => {
                var chain = Chain.Create("Teleport Sequence")
                    .Then(new TeleportChain(lifestream, aethernet))
                    .Then(GetMountChain);

                if (module.TryGetIPCProvider<VNavmesh>(out var vnav) && vnav != null && vnav.IsReady())
                {
                    chain.Then(() => GetPathfindingChain(vnav, ev, destination, 20f));
                }

                return chain;
            };

            ChainManager.Submit(factory);
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

        ChainManager.Submit(new ReturnChain(
            aetherytes[Svc.ClientState.TerritoryType],
            module.GetIPCProvider<YesAlready>(),
            module.GetIPCProvider<VNavmesh>()
        ));
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
