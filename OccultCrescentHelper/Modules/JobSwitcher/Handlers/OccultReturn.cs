using System;
using System.Collections.Generic;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin.Services;
using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;

namespace OccultCrescentHelper.JobSwitcher.Handlers;

public class OccultReturn : Handler
{
    private bool returning = false;
    private bool waitingForPopup = false;
    private bool waitingForTeleport = false;

    public OccultReturn(JobSwitcher switcher, List<MKDSupportJob> jobs)
        : base(switcher, jobs) { }

    public override void Enter()
    {
        returning = false;
        waitingForPopup = false;
        waitingForTeleport = false;
        ChangeToExpJob();
    }

    public override void Tick(IFramework _)
    {
        if (IsInCombat())
        {
            return;
        }

        var player = Svc.ClientState.LocalPlayer;
        if (player!.IsDead)
        {
            return;
        }

        unsafe
        {
            if (!returning)
            {
                if (player.StatusFlags.HasFlag(StatusFlags.WeaponOut))
                {
                    return;
                }

                ActionManager.Instance()->UseAction(ActionType.GeneralAction, 8);
                returning = true;
                waitingForPopup = true;
            }

            if (waitingForPopup)
            {
                var addon = Svc.GameGui.GetAddonByName("SelectYesno", 1);
                if (addon != IntPtr.Zero)
                {
                    var yesno = (AtkUnitBase*)addon;
                    if (yesno->IsReady)
                    {
                        Callback.Fire(yesno, true, 0);
                        waitingForPopup = false;
                        waitingForTeleport = true;
                    }
                }
                return;
            }

            if (waitingForTeleport)
            {
                if (player.IsCasting)
                {
                    return;
                }

                waitingForTeleport = false;
                switcher.SetState(JobSwitcherState.PostContent);
            }
        }
    }
}
