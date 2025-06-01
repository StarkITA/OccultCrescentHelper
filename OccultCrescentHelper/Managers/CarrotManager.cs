using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using OccultCrescentHelper.Enums;

namespace OccultCrescentHelper.Managers;

public class CarrotManager
{
    private static CarrotManager _instance;
    private static readonly object _lock = new object();
    public static List<IGameObject> carrots = [];

    // Private constructor to prevent instantiation
    private CarrotManager() { }

    // Public property to access the singleton instance
    public static CarrotManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new CarrotManager();
                }
            }

            return _instance;
        }
    }

    public static void UpdateCarrotList(IFramework framework)
    {
        if (!Helpers.IsInOccultCrescent())
        {
            return;
        }

        var pos = Svc.ClientState.LocalPlayer.Position;

        carrots = Svc
            .Objects.Where(o => o != null)
            .Where(o => o.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.EventObj)
            .Where(o => o.DataId == (uint)OccultObjectType.Carrot)
            .OrderBy(o => Vector3.Distance(o.Position, pos))
            .ToList();
    }
}
