using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin.Services;
using ECommons;
using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.EzIpcManager;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;

namespace OccultCrescentHelper.JobSwitcher.Handlers;

public class OccultReturn : Handler
{
    private enum ReturnState
    {
        Init,
        WaitingForYesno,
        WaitingForTeleport,
        WaitingToFinishTeleport,
        Done,
    }

    private enum TeleportState
    {
        Precast,
        Casting,
        Teleporting,
        Done,
        MovingToAetheryte,
    }

    private ReturnState state = ReturnState.Init;

    private TeleportState teleportState = TeleportState.Precast;

    private readonly Dictionary<uint, Vector3> aetherytes = new() { { 1252, new Vector3(830.75f, 72.98f, -695.98f) } };

    private readonly Dictionary<ReturnState, System.Action> stateHandlers;

    public OccultReturn(JobSwitcher switcher, List<MKDSupportJob> jobs)
        : base(switcher, jobs)
    {
        stateHandlers = new()
        {
            [ReturnState.Init] = HandleInitState,
            [ReturnState.WaitingForYesno] = HandleWaitingForYesnoState,
            [ReturnState.WaitingForTeleport] = HandleWaitingForTeleportState,
            [ReturnState.WaitingToFinishTeleport] = HandleWaitingToFinishTeleport,
            [ReturnState.Done] = () => { }, // no-op
        };
    }

    public override void Enter()
    {
        state = ReturnState.Init;
        teleportState = TeleportState.Precast;
        ChangeToExpJob();
    }

    public override void Tick(IFramework _)
    {
        if (IsInCombat())
            return;

        var player = Svc.ClientState.LocalPlayer;
        if (player!.IsDead)
            return;

        if (stateHandlers.TryGetValue(state, out var handler))
        {
            handler();
        }
    }

    private unsafe void HandleInitState()
    {
        var player = Svc.ClientState.LocalPlayer!;
        if (player.StatusFlags.HasFlag(StatusFlags.WeaponOut))
            return;

        ActionManager.Instance()->UseAction(ActionType.GeneralAction, 8);
        ChangeState(ReturnState.WaitingForYesno);
    }

    private unsafe void HandleWaitingForYesnoState()
    {
        var addon = Svc.GameGui.GetAddonByName("SelectYesno", 1);
        if (addon != IntPtr.Zero)
        {
            var yesno = (AtkUnitBase*)addon;
            if (yesno->IsReady)
            {
                Callback.Fire(yesno, true, 0);
                ChangeState(ReturnState.WaitingForTeleport);
            }
        }
    }

    private void HandleWaitingForTeleportState()
    {
        var player = Svc.ClientState.LocalPlayer!;
        if (player.IsCasting)
            return;

        if (!config.ApproachAetheryteAfterReturn)
        {
            ChangeState(ReturnState.Done);
            switcher.SetState(JobSwitcherState.PostContent);
        }

        ChangeState(ReturnState.WaitingToFinishTeleport);
        ChangeTeleportState(TeleportState.Casting);
    }

    private void HandleWaitingToFinishTeleport()
    {
        if (teleportState == TeleportState.Casting && Svc.Condition[ConditionFlag.BetweenAreas])
        {
            Svc.Log.Info("Teleporting");
            ChangeTeleportState(TeleportState.Teleporting);
        }

        if (teleportState == TeleportState.Teleporting && !Svc.Condition[ConditionFlag.BetweenAreas])
        {
            Svc.Log.Info("Done");
            ChangeTeleportState(TeleportState.Done);
        }

        if (teleportState == TeleportState.Done)
        {
            Svc.Log.Info("MovingToAetheryte");
            ChangeTeleportState(TeleportState.MovingToAetheryte);
            var pathfind = Svc.PluginInterface.GetIpcSubscriber<Vector3, bool, bool>("vnavmesh.SimpleMove.PathfindAndMoveTo");

            Random random = new();
            float angle = (float)(random.NextDouble() * Math.PI * 2);
            float distance = (float)(Math.Sqrt(random.NextDouble()) * (5f - 2f) + 2f);

            float xOffset = (float)Math.Cos(angle) * distance;
            float zOffset = (float)Math.Sin(angle) * distance;

            var aetheryte = aetherytes[Svc.ClientState.TerritoryType];
            var destination = new Vector3(aetheryte.X + xOffset, aetheryte.Y, aetheryte.Z + zOffset);
            Svc.Log.Info($"  Moving to aetheryte destination: {destination}");
            pathfind.InvokeFunc(destination, false);
        }
    }

    private void ChangeState(ReturnState newState)
    {
        Svc.Log.Info($"Changing state from {state} to {newState}");
        state = newState;
    }

    private void ChangeTeleportState(TeleportState newTeleportState)
    {
        Svc.Log.Info($"Changing teleport state from {teleportState} to {newTeleportState}");
        teleportState = newTeleportState;
    }
}
