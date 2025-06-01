using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using OccultCrescentHelper.Trackers;

namespace OccultCrescentHelper.Managers;

public class TreasureManager
{
    public static List<IGameObject> treasure = [];

    public static Vector4 bronze = new Vector4(
        (float)0.804,
        (float)0.498,
        (float)0.196,
        (float)1.0
    );

    public static Vector4 silver = new Vector4(
        (float)0.753,
        (float)0.753,
        (float)0.753,
        (float)1.0
    );

    public static Vector4 gold = new Vector4((float)1.0, (float)0.843, (float)0.0, (float)1.0);

    public static void UpdateTreasureList(IFramework framework)
    {
        treasure = Svc
            .Objects.OrderBy(o =>
                Vector3.Distance(o.Position, Svc.ClientState.LocalPlayer!.Position)
            )
            .Where(o => o.IsTargetable)
            .Where(o => o.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Treasure)
            .ToList();
    }
}
