using ECommons.DalamudServices;
using OccultCrescentHelper.Managers;

namespace OccultCrescentHelper.Overlay;

public class CarrotOverlay : IOverlayChild
{
    public void Draw(Overlay overlay)
    {
        var playerPosition = Svc.ClientState.LocalPlayer!.Position;
        foreach (var item in CarrotManager.carrots)
        {
            overlay.DrawLine(
                playerPosition,
                item.Position,
                5.0f,
                new System.Numerics.Vector4(0.93f, 0.57f, 0.13f, 1.0f)
            );
        }
    }
}
