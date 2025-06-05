using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using OccultCrescentHelper.Modules.CrowdSourcing;

namespace OccultCrescentHelper.Modules.Treasure;

public class TreasureTracker
{
    public List<Treasure> treasures { get; private set; } = [];

    public void Tick(IFramework _, Plugin plugin)
    {
        var pos = Svc.ClientState.LocalPlayer!.Position;

        treasures = Svc
            .Objects.Where(o => o != null)
            .Where(o => o.ObjectKind == ObjectKind.Treasure)
            .OrderBy(o => Vector3.Distance(o.Position, pos))
            .Select(o => new Treasure(o))
            .Where(t => t.IsValid())
            .ToList();

        var api = plugin.modules!.GetModule<CrowdSourcingModule>()!.api;
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
