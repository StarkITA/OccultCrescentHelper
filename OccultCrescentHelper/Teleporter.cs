using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.EzIpcManager;
using ECommons.GameHelpers;
using ECommons.Reflection;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using OccultCrescentHelper.Enums;

namespace OccultCrescentHelper;

public class Teleporter
{
    [EzIPC]
    public readonly Func<uint, bool> AethernetTeleportByPlaceNameId;

    private bool ready = false;

    private readonly Plugin plugin;

    private readonly Dictionary<Aethernet, Vector3> positions = new()
    {
        { Aethernet.TheWanderersHaven, new Vector3(-173.02f, 8.19f, -611.14f) },
        { Aethernet.CrystallizedCaverns, new Vector3(-358.14f, 101.98f, -120.96f) },
        { Aethernet.Eldergrowth, new Vector3(306.94f, 105.18f, 305.65f) },
        { Aethernet.Stonemarsh, new Vector3(-384.12f, 99.20f, 281.42f) },
    };

    public Teleporter(Plugin plugin)
    {
        this.plugin = plugin;
        ReadyCheck();
    }

    public void ReadyCheck()
    {
        if (!DalamudReflector.TryGetDalamudPlugin("Lifestream", out _, false, true))
        {
            return;
        }

        EzIPC.Init(this, "Lifestream", SafeWrapper.AnyException);
        ready = true;
    }

    public async Task Teleport(Aethernet aethernet)
    {
        if (!IsReady())
        {
            return;
        }

        AethernetTeleportByPlaceNameId((uint)aethernet);
        Svc.Log.Info("Waiting for tp to start...");
        await WaitForTeleportToStartAsync();
        Svc.Log.Info("Waiting for tp to finish...");
        await WaitForTeleportToFinishAsync();
        if (!plugin.config.TeleporterConfig.ShouldMount)
        {
            return;
        }

        Svc.Log.Info($"Mounting {plugin.config.TeleporterConfig.Mount}");

        await Task.Delay(500);
        Svc.Framework.RunOnFrameworkThread(() =>
        {
            unsafe
            {
                ActionManager.Instance()->UseAction(ActionType.Mount, plugin.config.TeleporterConfig.Mount);
            }
        });
    }

    private async Task WaitForTeleportToStartAsync(int timeout = 5000)
    {
        int waited = 0;
        const int interval = 250;

        while (!Svc.Condition[ConditionFlag.BetweenAreas] && waited < timeout)
        {
            await Task.Delay(interval);
            waited += interval;
        }
    }

    private async Task WaitForTeleportToFinishAsync(int timeout = 5000)
    {
        int waited = 0;
        const int interval = 250;

        while (Svc.Condition[ConditionFlag.BetweenAreas] && waited < timeout)
        {
            await Task.Delay(interval);
            waited += interval;
        }
    }

    public Aethernet GetClosestAethernet(Vector3 position)
    {
        Aethernet? closest = null;
        float minDistance = float.MaxValue;

        foreach (var kvp in positions)
        {
            float distance = Vector3.Distance(position, kvp.Value);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = kvp.Key;
            }
        }

        return (Aethernet)closest!;
    }

    public bool IsReady() => ready;
}
