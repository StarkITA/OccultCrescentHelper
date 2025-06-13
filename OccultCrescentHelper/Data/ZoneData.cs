using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ECommons.DalamudServices;

namespace OccultCrescentHelper.Data;

public static class ZoneData
{
    public const uint SOUTHHORN = 1252;

    public static readonly List<uint> zoneIds = [
        SOUTHHORN
    ];

    // This can and should be filled using layout files or excel data
    public static readonly Dictionary<uint, Vector3> aetherytes = new() {
        { SOUTHHORN, new Vector3(830.75f, 72.98f, -695.98f) }
    };

    public static readonly Dictionary<uint, Vector3> startingLocations = new() {
        { SOUTHHORN, new Vector3(850.33f, 72.99f, -704.07f) }
    };

    // Zone functions
    public static bool IsInSouthHorn()
    {
        return Svc.ClientState.TerritoryType == ZoneData.SOUTHHORN;
    }

    public static bool IsInOccultCrescent()
    {
        return Svc.ClientState.LocalPlayer != null && IsInSouthHorn();
    }

    // Tower functions
    public static bool IsInForkedTowerBlood()
    {
        var player = Svc.ClientState.LocalPlayer;
        if (player == null)
        {
            return false;
        }


        List<uint> statuses = [
            (uint)Status.DutiesAsAssigned,
            (uint)Status.ResurrectionDenied,
            (uint)Status.ResurrectionRestricted
        ];

        return player.StatusList.Any(s => statuses.Contains(s.StatusId));
    }

    public static bool IsInForkedTower()
    {
        return IsInForkedTowerBlood();
    }
}
