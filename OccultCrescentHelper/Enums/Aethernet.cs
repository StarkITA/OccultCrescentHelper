using System.Linq;
using ECommons.DalamudServices;
using Lumina.Excel.Sheets;

namespace OccultCrescentHelper.Enums;

public enum Aethernet : uint
{
    // -173.02 8.19 -611.14
    TheWanderersHaven = 4936,

    // -358.14 101.98 -120.96
    CrystallizedCaverns = 4929,

    // 306.94 105.18 305.65
    Eldergrowth = 4930,

    // -384.12 99.20 281.42
    Stonemarsh = 4942,
}

public static class AethernetExtensions
{
    public static string ToFriendlyString(this Aethernet aethernet)
    {
        return Svc.Data.GetExcelSheet<PlaceName>().FirstOrDefault(p => p.RowId == (uint)aethernet).Name.ToString();
    }
}
