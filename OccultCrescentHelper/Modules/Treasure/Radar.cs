using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using OccultCrescentHelper.Enums;

namespace OccultCrescentHelper.Modules.Treasure;

public class Radar
{
    public void Draw(TreasureModule module)
    {
        if (!Helpers.IsInOccultCrescent())
        {
            return;
        }

        if (!module.config.DrawLineToBronzeChests && !module.config.DrawLineToSilverChests)
        {
            return;
        }

        if (Svc.ClientState.LocalPlayer == null || Svc.Condition[ConditionFlag.InCombat])
        {
            return;
        }

        var pos = Svc.ClientState.LocalPlayer!.Position;

        foreach (var treasure in module.treasures)
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
