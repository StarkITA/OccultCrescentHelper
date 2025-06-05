using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using OccultCrescentHelper.Enums;
using Ocelot.Chain;
using Ocelot.IPC;

namespace OccultCrescentHelper.Modules.Teleporter;

public class Teleporter
{
    private readonly TeleporterModule module;

    private readonly Dictionary<Aethernet, Vector3> positions = new()
    {
        { Aethernet.TheWanderersHaven, new Vector3(-173.02f, 8.19f, -611.14f) },
        { Aethernet.CrystallizedCaverns, new Vector3(-358.14f, 101.98f, -120.96f) },
        { Aethernet.Eldergrowth, new Vector3(306.94f, 105.18f, 305.65f) },
        { Aethernet.Stonemarsh, new Vector3(-384.12f, 99.20f, 281.42f) },
    };

    private readonly HashSet<uint> validAethernetShards = new() {
        2014664, // Basecamp
        2014665, // Beach
        2014666, // Cave
        2014667, // Citadel
        2014744, // Swamp
    };

    public Teleporter(TeleporterModule module)
    {
        this.module = module;
    }

    public async Task Teleport(Aethernet aethernet, Vector3? destination = null)
    {
        if (module.plugin.ipc?.TryGetProvider<Lifestream>(out var ipc) ?? false)
        {
            var chain = ChainBuilder.Begin()
                .Then(() => ipc!.AethernetTeleportByPlaceNameId!((uint)aethernet))
                .WaitUntil(() => Svc.Condition[ConditionFlag.BetweenAreas])
                .WaitWhile(() => Svc.Condition[ConditionFlag.BetweenAreas])
                .BreakIf(() => !module.config.ShouldMount)
                .Wait(500)
                .ThenOnFrameworkThread(() => Mount())
                .BreakIf(() => !module.config.PathToDestination || destination == null || destination == Vector3.Zero)
                .Then(() => {
                    if (module.TryGetIPCProvider<VNavmesh>(out var vnav) && vnav!.IsReady())
                    {
                        vnav.PathfindAndMoveTo((Vector3)destination, false);
                    }
                })
                .Build();

            await ChainRunner.Run(chain);
        }
    }

    public unsafe void Mount() => ActionManager.Instance()->UseAction(ActionType.Mount, module.config.Mount);

    public Aethernet GetClosestAethernet(Vector3 position)
    {
        Aethernet? closest = null;
        float minDistance = float.MaxValue;

        foreach (var kvp in positions)
        {
            float distance = Vector3.Distance(position, kvp.Value);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = kvp.Key;
            }
        }

        return (Aethernet)closest!;
    }

    public bool IsReady()
    {
        if (module.TryGetIPCProvider<Lifestream>(out var ipc))
        {
            var playerPos = Svc.ClientState.LocalPlayer?.Position ?? Vector3.Zero;

            return Svc.Objects
                .Where(o => o.ObjectKind == ObjectKind.EventObj)
                .Where(o => validAethernetShards.Contains(o.DataId))
                .Where(o => Vector3.Distance(o.Position, playerPos) <= 4.5f)
                .OrderBy(o => Vector3.DistanceSquared(o.Position, playerPos))
                .Count() > 0;
        }

        return false;
    }
}
