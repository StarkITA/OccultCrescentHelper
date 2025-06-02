using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using OccultCrescentHelper.Api;

namespace OccultCrescentHelper.Treasure;

public class TreasureTracker
{
    public List<Treasure> treasures = [];

    private CrowdSourcingApi api;

    public TreasureTracker(CrowdSourcingApi api)
    {
        this.api = api;
    }

    public void Tick(IFramework _)
    {
        var pos = Svc.ClientState.LocalPlayer!.Position;

        treasures = Svc
            .Objects.Where(o => o != null)
            .Where(o => o.ObjectKind == ObjectKind.Treasure)
            .OrderBy(o => Vector3.Distance(o.Position, pos))
            .Select(o => new Treasure(o))
            .ToList();

        foreach (var treasure in treasures)
        {
            if (!treasure.IsValid())
            {
                continue;
            }

            api.SendTreasure(treasure.GetPosition(), treasure.GetModelId());
        }
    }
}
