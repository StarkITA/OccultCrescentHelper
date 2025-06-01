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
            if (item == null || item.IsDead || !item.IsValid())
            {
                continue;
            }

            var data = Svc
                .Data.GetExcelSheet<Treasure>()
                .ToList()
                .FirstOrDefault(t => t.RowId == item.DataId);

            var isSilver = data.SGB.RowId == 1597;

            var render = false;
            if (isSilver)
            {
                render = overlay.plugin.config.DrawLineToSilverChests;
            }
            else
            {
                render = overlay.plugin.config.DrawLineToBronzeChests;
            }

            if (!render)
            {
                continue;
            }

            var color = isSilver ? TreasureManager.silver : TreasureManager.bronze;

            overlay.DrawLine(playerPosition, item.Position, 5.0f, color);
        }
    }
}
