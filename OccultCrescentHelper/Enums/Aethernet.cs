using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using Lumina.Excel.Sheets;

namespace OccultCrescentHelper.Enums;

public enum Aethernet : uint
{
    // 830.75 72.98 -695.98
    BaseCamp = 4944,
    // -173.02 8.19 -611.14
    TheWanderersHaven = 4936,

    // -358.14 101.98 -120.96
    CrystallizedCaverns = 4929,

    // 306.94 105.18 305.65
    Eldergrowth = 4930,

    // -384.12 99.20 281.42
    Stonemarsh = 4942,
}

public class AethernetData
{
    public Aethernet aethernet;

    public uint dataId;

    public Vector3 position;

    public static List<AethernetData> All()
    {
        return ((Aethernet[])System.Enum.GetValues(typeof(Aethernet)))
            .Select(a => a.GetData())
            .ToList();
    }



    public static AethernetData GetClosestTo(Vector3 to)
    {
        return All().OrderBy((data) => Vector3.Distance(to, data.position)).First();
    }

    public static AethernetData GetClosestToPlayer()
    {
        return GetClosestTo(Player.Position);
    }

    public float Distance()
    {
        return Vector3.Distance(Player.Position, position);
    }
}

public static class AethernetExtensions
{
    public static string ToFriendlyString(this Aethernet aethernet)
    {
        return Svc.Data.GetExcelSheet<PlaceName>().FirstOrDefault(p => p.RowId == (uint)aethernet).Name.ToString();
    }

    public static AethernetData GetData(this Aethernet aethernet)
    {
        switch (aethernet)
        {
            case Aethernet.BaseCamp:
                return new AethernetData { aethernet = Aethernet.BaseCamp, dataId = 2014664, position = new Vector3(830.75f, 72.98f, -695.98f) };
            case Aethernet.TheWanderersHaven:
                return new AethernetData { aethernet = Aethernet.TheWanderersHaven, dataId = 2014665, position = new Vector3(-173.02f, 8.19f, -611.14f) };
            case Aethernet.CrystallizedCaverns:
                return new AethernetData { aethernet = Aethernet.CrystallizedCaverns, dataId = 2014666, position = new Vector3(-358.14f, 101.98f, -120.96f) };
            case Aethernet.Eldergrowth:
                return new AethernetData { aethernet = Aethernet.Eldergrowth, dataId = 2014667, position = new Vector3(306.94f, 105.18f, 305.65f) };
            case Aethernet.Stonemarsh:
                return new AethernetData { aethernet = Aethernet.Stonemarsh, dataId = 2014744, position = new Vector3(-384.12f, 99.20f, 281.42f) };
            default:
                return new AethernetData { aethernet = Aethernet.BaseCamp, dataId = 2014664, position = new Vector3(830.75f, 72.98f, -695.98f) };
        }
    }
}
