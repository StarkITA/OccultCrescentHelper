using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using Lumina.Excel.Sheets;

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
        if (!Helpers.IsInOccultCrescent())
        {
            return;
        }

        var pos = Svc.ClientState.LocalPlayer.Position;

        treasure = Svc
            .Objects.Where(o => o != null)
            .Where(o => o.IsTargetable)
            .Where(o => o.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Treasure)
            .OrderBy(o => Vector3.Distance(o.Position, pos))
            .ToList();

        foreach (var item in treasure)
        {
            if (item == null || item.IsDead || !item.IsValid())
            {
                continue;
            }

            var data = Svc
                .Data.GetExcelSheet<Treasure>()
                .ToList()
                .FirstOrDefault(t => t.RowId == item.DataId);

            var isSilver = data.SGB.RowId == 1597;

            if (isSilver)
            {
                Api.SendSilverTreasure(item.Position, Plugin.Instance.config);
            }
            else
            {
                Api.SendBronzeTreasure(item.Position, Plugin.Instance.config);
            }
        }
    }
}
