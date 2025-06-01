using ECommons.DalamudServices;

namespace OccultCrescentHelper;

public static class Helpers
{
    public static bool IsInSouthHorn()
    {
        return Svc.ClientState.TerritoryType == 1252;
    }

    public static bool IsInOccultCrescent()
    {
        return IsInSouthHorn() && Svc.ClientState.LocalPlayer != null;
    }
}
