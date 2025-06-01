using System.Linq;
using ECommons.DalamudServices;
using Lumina.Excel.Sheets;
using OccultCrescentHelper.Managers;

namespace OccultCrescentHelper.Overlay;

public class TreasureOverlay : IOverlayChild
{
    public void Draw(Overlay overlay)
    {
        var playerPosition = Svc.ClientState.LocalPlayer!.Position;
        foreach (var item in TreasureManager.treasure)
        {
            var data = Svc
                .Data.GetExcelSheet<Treasure>()
                .ToList()
                .FirstOrDefault(t => t.RowId == item.DataId);

            var color = data.SGB.RowId == 1597 ? TreasureManager.silver : TreasureManager.bronze;

            overlay.DrawLine(playerPosition, item.Position, 5.0f, color);
        }
    }
}
