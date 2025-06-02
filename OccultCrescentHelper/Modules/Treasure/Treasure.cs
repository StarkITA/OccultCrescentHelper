using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using OccultCrescentHelper.Enums;
using XIVTreasure = Lumina.Excel.Sheets.Treasure;

namespace OccultCrescentHelper.Treasure;

public class Treasure
{
    private readonly IGameObject gameObject;

    public Treasure(IGameObject obj)
    {
        gameObject = obj;
    }

    private XIVTreasure? GetData() => Svc.Data.GetExcelSheet<XIVTreasure>().ToList().FirstOrDefault(t => t.RowId == gameObject.DataId);

    public bool IsValid() => gameObject != null && !gameObject.IsDead && gameObject.IsValid();

    public Vector3 GetPosition() => gameObject.Position;

    public uint? GetModelId() => GetData()?.SGB.RowId;

    public TreasureType GetTreasureType()
    {
        var model = GetModelId();
        if (model == 1597)
        {
            return TreasureType.Silver;
        }

        if (model == 1596)
        {
            return TreasureType.Bronze;
        }

        return TreasureType.Unknown;
    }

    public Vector4 GetColor()
    {
        switch (GetTreasureType())
        {
            case TreasureType.Bronze:
                return TreasureModule.bronze;
            case TreasureType.Silver:
                return TreasureModule.silver;
            default:
                return TreasureModule.unknown;
        }
    }

    public string GetName()
    {
        switch (GetTreasureType())
        {
            case TreasureType.Bronze:
                return "Bronze Treasure Coffer";
            case TreasureType.Silver:
                return "Silver Treasure Coffer";
            default:
                return "Unknown Treasure Coffer";
        }
    }

    public void Target() => Svc.Targets.Target = gameObject;
}
