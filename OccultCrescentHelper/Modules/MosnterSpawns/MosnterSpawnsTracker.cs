using System;
using System.Linq;
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
    }
}
