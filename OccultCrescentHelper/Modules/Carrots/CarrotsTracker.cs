using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using OccultCrescentHelper.Api;
using OccultCrescentHelper.Enums;

namespace OccultCrescentHelper.Carrots;

public class CarrotsTracker
{
    public List<Carrot> carrots = [];

    private CrowdSourcingApi api;

    public CarrotsTracker(CrowdSourcingApi api)
    {
        this.api = api;
    }

    public void Tick(IFramework _)
    {
        var pos = Svc.ClientState.LocalPlayer!.Position;

        carrots = Svc
            .Objects.Where(o => o != null)
            .Where(o => o.ObjectKind == ObjectKind.EventObj)
            .Where(o => o.DataId == (uint)OccultObjectType.Carrot)
            .OrderBy(o => Vector3.Distance(o.Position, pos))
            .Select(o => new Carrot(o))
            .ToList();

        foreach (var carrot in carrots)
        {
            if (!carrot.IsValid())
            {
                continue;
            }

            api.SendCarrot(carrot.GetPosition());
        }
    }
}
