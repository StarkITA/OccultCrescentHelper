using ECommons.DalamudServices;

namespace OccultCrescentHelper.Carrots;

public class Radar
{
    private CarrotsModule module;

    public Radar(CarrotsModule module)
    {
        this.module = module;
    }

    public void Draw()
    {
        if (!module.config.DrawLineToCarrots)
        {
            return;
        }

        var pos = Svc.ClientState.LocalPlayer!.Position;

        foreach (var carrot in module.tracker.carrots)
        {
            if (!carrot.IsValid())
            {
                continue;
            }

            Helpers.DrawLine(pos, carrot.GetPosition(), 3f, Carrot.color);
        }
    }
}
