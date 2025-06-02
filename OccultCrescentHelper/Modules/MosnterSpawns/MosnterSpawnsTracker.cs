using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using OccultCrescentHelper.Api;

namespace OccultCrescentHelper.MosnterSpawns;

public class MosnterSpawnsTracker
{
    private CrowdSourcingApi api;

    private float elapsed = 0f;

    private float interval = 3f;

    public MosnterSpawnsTracker(CrowdSourcingApi api)
    {
        this.api = api;
    }

    public void Tick(IFramework framework)
    {
        elapsed += framework.UpdateDelta.Milliseconds / 1000f;
        if (elapsed < interval)
        {
            return;
        }

        elapsed -= interval;
        var pos = Svc.ClientState.LocalPlayer!.Position;

        foreach (var npc in Svc.Objects.OfType<IBattleNpc>())
        {
            IntPtr ptr = npc.Address;
            unsafe
            {
                GameObject* obj = (GameObject*)ptr.ToPointer();
                var spawn = obj->DefaultPosition;

                MonsterPayload payload = new MonsterPayload()
                {
                    name = npc.Name.ToString(),
                    spawn_position = new Position()
                    {
                        X = spawn.X,
                        Y = spawn.Y,
                        Z = spawn.Z,
                    },
                };

                api.SendMonsterSpawn(payload);
            }
        }

        // var treasures = Svc
        //     .Objects.Where(o => o != null)
        //     .Where(o => o.ObjectKind == ObjectKind.Treasure)
        //     .OrderBy(o => Vector3.Distance(o.Position, pos))
        //     .Select(o => new Treasure(o))
        //     .ToList();

        // foreach (var treasure in treasures)
        // {
        //     if (!treasure.IsValid())
        //     {
        //         continue;
        //     }

        //     api.SendTreasure(treasure.GetPosition(), treasure.GetModelId());
        // }
    }
}
