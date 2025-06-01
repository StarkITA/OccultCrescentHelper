using ECommons.DalamudServices;
using OccultCrescentHelper.Managers;

namespace OccultCrescentHelper.Overlay;

public class TreasureOverlay : IOverlayChild
{
    public void Draw(Overlay overlay)
    {
        var playerPosition = Svc.ClientState.LocalPlayer!.Position;
        foreach (var item in TreasureManager.treasure)
        {
            overlay.DrawLine(playerPosition, item.Position, 5.0f, TreasureManager.bronze);

            Svc.Log.Info(item.SubKind.ToString());
        }
    }
}
