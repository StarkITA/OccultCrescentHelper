using ECommons.DalamudServices;
using OccultCrescentHelper.Enums;

namespace OccultCrescentHelper.Treasure;

public class Radar
{
    private TreasureModule module;

    public Radar(TreasureModule module)
    {
        this.module = module;
    }

    public void Draw()
    {
        if (!module.config.DrawLineToBronzeChests && !module.config.DrawLineToSilverChests)
        {
            return;
        }

        var pos = Svc.ClientState.LocalPlayer!.Position;

        foreach (var treasure in module.tracker.treasures)
        {
            if (!treasure.IsValid())
            {
                continue;
            }

            switch (treasure.GetTreasureType())
            {
                case TreasureType.Bronze:
                    if (module.config.DrawLineToBronzeChests)
                    {
                        Helpers.DrawLine(pos, treasure.GetPosition(), 3f, treasure.GetColor());
                    }
                    break;

                case TreasureType.Silver:
                    if (module.config.DrawLineToSilverChests)
                    {
                        Helpers.DrawLine(pos, treasure.GetPosition(), 3f, treasure.GetColor());
                    }
                    break;
            }
        }
    }
}
