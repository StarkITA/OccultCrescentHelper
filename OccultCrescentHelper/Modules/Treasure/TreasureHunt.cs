
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin.Services;
using ECommons.Automation.LegacyTaskManager;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using ImGuiNET;
using OccultCrescentHelper.Chains;
using OccultCrescentHelper.Data;
using OccultCrescentHelper.Enums;
using OccultCrescentHelper.Modules.Debug.Panels;
using OccultCrescentHelper.Modules.Mount.Chains;
using Ocelot;
using Ocelot.Chain;
using Ocelot.Chain.ChainEx;
using Ocelot.IPC;
using Ocelot.Modules;
using Ocelot.Prowler;

namespace OccultCrescentHelper.Modules.Treasure;

public class TreasureHunt
{
    private const float INTERACT_THRESHOLD = 2f;

    private const float DATA_THRESHOLD = 100f;

    private List<List<Vector3>> paths = [
        [
            new Vector3(617.09f, 66.3f, -703.88f),
            new Vector3(490.41f, 62.46f, -590.57f),
            new Vector3(666.53f, 79.12f, -480.37f),
            new Vector3(870.66f, 95.69f, -388.36f),
            new Vector3(779.02f, 96.09f, -256.24f),
            new Vector3(770.75f, 107.99f, -143.57f),
            new Vector3(726.28f, 108.14f, -67.92f),
            new Vector3(788.88f, 120.38f, 109.39f),
            new Vector3(609.61f, 107.99f, 117.27f),
            new Vector3(475.73f, 95.99f, -87.08f),
            new Vector3(245.59f, 109.12f, -18.17f),
            new Vector3(354.12f, 95.66f, -288.93f),
            new Vector3(386.92f, 96.79f, -451.38f),
            new Vector3(381.73f, 22.17f, -743.65f),
            new Vector3(142.11f, 16.4f, -574.06f),
            new Vector3(-118.97f, 4.99f, -708.46f),
            new Vector3(-140.46f, 22.35f, -414.27f),
            new Vector3(-343.16f, 52.32f, -382.13f),
            new Vector3(-491.02f, 2.98f, -529.59f),
            new Vector3(-451.68f, 2.98f, -775.57f),
            new Vector3(-585.29f, 4.99f, -864.84f),
            new Vector3(-729.43f, 4.99f, -724.82f),
            new Vector3(-825.16f, 2.98f, -832.27f),
            new Vector3(-661.71f, 2.98f, -579.49f),
        ],
        [
            new Vector3(277.79f, 103.78f, 241.9f),
            new Vector3(517.75f, 67.89f, 236.13f),
            new Vector3(642.97f, 69.99f, 407.8f),
            new Vector3(697.32f, 69.99f, 597.92f),
            new Vector3(835.08f, 69.99f, 699.09f),
            new Vector3(596.46f, 70.3f, 622.77f),
            new Vector3(471.18f, 70.3f, 530.02f),
            new Vector3(256.15f, 73.17f, 492.36f),
            new Vector3(294.88f, 56.08f, 640.22f),
            new Vector3(433.71f, 70.3f, 683.53f),
            new Vector3(140.98f, 55.99f, 770.99f),
            new Vector3(35.72f, 65.11f, 648.98f),
            new Vector3(-225.02f, 75f, 804.99f),
            new Vector3(-197.19f, 74.91f, 618.34f),
            new Vector3(-372.67f, 75f, 527.43f),
            new Vector3(-648f, 75f, 403.95f),
            new Vector3(-401.66f, 85.04f, 332.54f),
            new Vector3(-283.99f, 115.98f, 377.04f),
            new Vector3(-256.89f, 120.99f, 125.08f),
            new Vector3(-25.68f, 102.22f, 150.16f),
            new Vector3(8.99f, 103.2f, 426.96f),
        ],
        [
            new Vector3(-158.65f, 98.62f, -132.74f),
            new Vector3(-487.11f, 98.53f, -205.46f),
            new Vector3(-444.11f, 90.68f, 26.23f),
            new Vector3(-394.89f, 106.74f, 175.43f),
            new Vector3(-682.8f, 135.61f, -195.27f),
            new Vector3(-729.92f, 116.53f, -79.06f),
            new Vector3(-756.83f, 76.55f, 97.37f),
            new Vector3(-713.8f, 62.06f, 192.61f),
        ]
    ];

    private int pathIndex = 0;

    private List<Vector3> currentPath => paths[pathIndex];

    private bool isFinalPath => pathIndex >= paths.Count - 1;

    private int nodeIndex = 0;

    private Vector3 currentNode => currentPath[nodeIndex];

    private bool isFinalNode => nodeIndex >= currentPath.Count - 1;

    private bool running = false;

    private volatile float distance = 0f;

    public void Tick(TreasureModule module)
    {
        if (!running)
        {
            return;
        }

        if (!module.TryGetIPCProvider<VNavmesh>(out var vnav) || vnav == null)
        {
            return;
        }

        if (!module.TryGetIPCProvider<Lifestream>(out var lifestream) || lifestream == null)
        {
            return;
        }

        MaintainWatcherChain(module, vnav, lifestream);
    }

    private bool Is(Vector3 a, Vector3 b, float variance = 1f)
    {
        return Vector3.Distance(a, b) <= variance;
    }

    private void MaintainWatcherChain(TreasureModule module, VNavmesh vnav, Lifestream lifestream)
    {
        if (Plugin.Chain.IsRunning)
        {
            return;
        }

        Plugin.Chain.Submit(
            () => Chain.Create($"Treasure hunt looper ({pathIndex + 1}:{nodeIndex + 1})")
                .Then(new TaskManagerTask(() => {
                    if (!vnav.IsRunning())
                    {
                        vnav.PathfindAndMoveTo(currentNode, false);
                    }

                    var treasures = Svc.Objects
                        .Where(o => o?.ObjectKind == ObjectKind.Treasure && o.IsValid() && !o.IsDead && o.IsTargetable)
                        .ToList();

                    var distance = Vector3.Distance(Player.Position, currentNode);
                    this.distance = distance;
                    if (distance <= DATA_THRESHOLD)
                    {
                        ;
                        if (treasures.Any(o => Is(o.Position, currentNode)))
                        {
                            if (distance > INTERACT_THRESHOLD)
                            {
                                return false;
                            }

                            Svc.Targets.Target = treasures.First();

                            vnav.Stop();
                        }

                        if (isFinalNode)
                        {
                            if (isFinalPath)
                            {
                                running = false;
                                return true;
                            }

                            pathIndex++;
                            nodeIndex = 0;
                            OnPathChanged(module, vnav, lifestream);
                            return true;
                        }

                        nodeIndex++;
                        vnav.PathfindAndMoveTo(currentNode, false);
                        return true;
                    }

                    return false;
                }, new() { TimeLimitMS = int.MaxValue })
            )
            .ConditionalWait(_ => Svc.Targets.Target != null, 1000)
        );
    }

    public void Draw(TreasureModule module)
    {
        if (!module.TryGetIPCProvider<VNavmesh>(out var vnav) || vnav == null)
        {
            return;
        }

        if (!module.TryGetIPCProvider<Lifestream>(out var lifestream) || lifestream == null)
        {
            return;
        }

        OcelotUI.Title("Treasure Hunter:");
        OcelotUI.Indent(() => {
            if (ImGui.Button(running ? "Stop" : "Start"))
            {
                pathIndex = 0;
                nodeIndex = 0;
                running = !running;
                distance = 0f;
                if (running == false)
                {
                    vnav.Stop();
                    Plugin.Chain.Abort();
                }
                else
                {
                    OnPathChanged(module, vnav, lifestream);
                }
            }

            if (running)
            {
                OcelotUI.Title("Distance to next node:");
                ImGui.SameLine();
                ImGui.TextUnformatted(distance.ToString());

                var instances = ChainManager.Active();
                OcelotUI.Title("# of instances:");
                ImGui.SameLine();
                ImGui.TextUnformatted(instances.Count.ToString());

                foreach (var pair in instances)
                {
                    if (pair.Value.CurrentChain == null)
                    {
                        continue;
                    }

                    OcelotUI.Title($"{pair.Key}:");
                    OcelotUI.Indent(() => {
                        var current = pair.Value.CurrentChain!;
                        OcelotUI.Title("Current Chain:");
                        ImGui.SameLine();
                        ImGui.TextUnformatted(current.name);

                        OcelotUI.Title("Progress:");
                        ImGui.SameLine();
                        ImGui.TextUnformatted($"{current.progress * 100}%");

                        OcelotUI.Title("Queued Chains:");
                        ImGui.SameLine();
                        ImGui.TextUnformatted(pair.Value.QueueCount.ToString());
                    });
                }
            }
        });
    }

    private void OnPathChanged(TreasureModule module, VNavmesh vnav, Lifestream lifestream)
    {
        vnav.Stop();
        Plugin.Chain.Abort();
        switch (pathIndex)
        {
            case 0:
                module.Warning("0:start");
                Plugin.Chain.Submit(
                    () => Chain.Create("Treasure hunt phase 1 setup")
                        .Then(new ReturnChain(new Vector3(804.38f, 70.86f, -691.59f), module.GetIPCProvider<YesAlready>(), module.GetIPCProvider<VNavmesh>()))
                        .Then(new MountChain(module._config.MountConfig))
                );
                break;
            case 1:
                module.Warning("1:start");
                Plugin.Chain.Submit(
                    () => Chain.Create("Treasure hunt phase 2 setup")
                        .ConditionalThen(_ => Svc.Condition[ConditionFlag.InCombat], _ => new TaskManagerTask(() => {
                            if (!vnav.IsRunning())
                            {
                                vnav.PathfindAndMoveTo(new Vector3(-173.02f, 8.19f, -611.14f), false);
                            }

                            return !Svc.Condition[ConditionFlag.InCombat];
                        }, new() { TimeLimitMS = int.MaxValue }))
                        .Then(_ => vnav.Stop())
                        .Then(new ReturnChain(Aethernet.BaseCamp.GetData().position, module.GetIPCProvider<YesAlready>(), module.GetIPCProvider<VNavmesh>()))
                        .WaitForPathfindingCycle(vnav)
                        .Then(new TeleportChain(lifestream, Enums.Aethernet.Eldergrowth))
                        .Then(new MountChain(module._config.MountConfig))
                );
                break;
            case 2:
                module.Warning("2:start");
                Plugin.Chain.Submit(
                    () => Chain.Create("Treasure hunt phase 3 setup")
                        .ConditionalThen(_ => Svc.Condition[ConditionFlag.InCombat], _ => new TaskManagerTask(() => {
                            if (!vnav.IsRunning())
                            {
                                vnav.PathfindAndMoveTo(new Vector3(-173.02f, 8.19f, -611.14f), false);
                            }

                            return !Svc.Condition[ConditionFlag.InCombat];
                        }, new() { TimeLimitMS = int.MaxValue }))
                        .Then(_ => vnav.Stop())
                        .Then(new ReturnChain(Aethernet.BaseCamp.GetData().position, module.GetIPCProvider<YesAlready>(), module.GetIPCProvider<VNavmesh>()))
                        .WaitForPathfindingCycle(vnav)
                        .Then(new TeleportChain(lifestream, Enums.Aethernet.CrystallizedCaverns))
                        .Then(new MountChain(module._config.MountConfig))
                );
                break;
        }
    }
}
