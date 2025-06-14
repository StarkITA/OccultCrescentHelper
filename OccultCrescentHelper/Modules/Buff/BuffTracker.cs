using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ECommons;
using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Common.Math;
using OccultCrescentHelper.Enums;

namespace OccultCrescentHelper.Modules.Buff;

public class BuffTracker
{
    public BuffTracker()
    {
        Reset();
    }

    public void Tick(IFramework _)
    {
        //     getNearbyCrystal();
    }

    public void ResetBuff()
    {
        SwitchToBard();
        SwitchToMonk();
        SwitchToPaladin();
    }

    public void Reset() { }

    public void debug()
    {
        ResetBardBuff();
    }

    public bool IsNearCrystal()
    {
        return NearbyCrystalCount() > 0;
    }

    public int NearbyCrystalCount()
    {
        var playerPos = Svc.ClientState.LocalPlayer?.Position ?? Vector3.Zero;

        return Svc.Objects
                  .Where(o => o.ObjectKind == ObjectKind.EventObj)
                  .Where(o => o.DataId == 2007457)
                  .Count(o => Vector3.Distance(o.Position, playerPos) <= 4.5f);
    }

    public void OpenSupportJob()
    {
        Svc.Log.Debug("Trying to open the support job GUI.");
        unsafe
        {
            var baseSupportGui = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MKDInfo");
            if (!IsSupportGuiVisible())
            {
                var task = Svc.Framework.RunOnFrameworkThread(() => {
                    Callback.Fire(baseSupportGui, true, 1, 0);
                });
                task.Wait();
                OpenSupportJob();
                Svc.Log.Debug("Support job callback sent");
            }
        }
    }

    private unsafe bool IsSupportGuiVisible()
    {
        var supportJobAddon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MKDSupportJob");
        var isSupportGuiVisible = supportJobAddon != null && supportJobAddon->IsVisible;
        if (isSupportGuiVisible)
        {
            Svc.Log.Debug("Support job visible.");
        }
        else
        {
            Svc.Log.Debug("Support job not visible.");
        }

        return isSupportGuiVisible;
    }

    public void OpenSupportJobList()
    {
        Svc.Log.Debug("Trying to open support job list.");
        if (!IsSupportJobListVisible() && !IsSupportGuiVisible())
        {
            OpenSupportJob();
        }

        unsafe
        {
            var supportJobListAddon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MKDSupportJob");
            if (!IsSupportJobListVisible())
            {
                var task = Svc.Framework.RunOnFrameworkThread(() => {
                    Callback.Fire(supportJobListAddon, true, 0, 0, 0);
                    Svc.Log.Debug("Job List callback sent");
                });
                task.WaitSafely();
                OpenSupportJobList();
            }
            else
            {
                Svc.Log.Debug("Support job list already visible");
            }
        }
    }

    private unsafe bool IsSupportJobListVisible()
    {
        var supportJobListAddon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MKDSupportJobList");
        var isSupportJobListVisible = supportJobListAddon != null && supportJobListAddon->IsVisible;
        if (isSupportJobListVisible)
        {
            Svc.Log.Debug("Support job list visible.");
        }
        else
        {
            Svc.Log.Debug("Support job list not visible.");
        }

        return isSupportJobListVisible;
    }

    public void SwitchToBard()
    {
        if (IsBard())
        {
            return;
        }

        if (IsSupportJobListVisible())
        {
            SwitchToClassId(6);
            SwitchToBard();
        }
        else
        {
            if (IsSupportGuiVisible())
            {
                OpenSupportJobList();
                SwitchToBard();
            }
            else
            {
                OpenSupportJob();
                SwitchToBard();
            }
        }
    }

    public void SwitchToMonk()
    {
        if (IsMonk())
        {
            return;
        }

        if (IsSupportJobListVisible())
        {
            SwitchToClassId(3);
        }
        else
        {
            if (IsSupportGuiVisible())
            {
                OpenSupportJobList();
                SwitchToMonk();
            }
            else
            {
                OpenSupportJob();
                SwitchToMonk();
            }
        }
    }

    public void SwitchToPaladin()
    {
        if (IsPaladin())
        {
            return;
        }

        if (IsSupportJobListVisible())
        {
            SwitchToClassId(1);
        }
        else
        {
            if (IsSupportGuiVisible())
            {
                OpenSupportJobList();
                SwitchToPaladin();
            }
            else
            {
                OpenSupportJob();
                SwitchToPaladin();
            }
        }
    }

    public void SwitchToClassId(uint classId)
    {
        Svc.Log.Debug($"Switching to class {classId}");
        unsafe
        {
            if (IsSupportJobListVisible())
            {
                var supportJobListAddon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MKDSupportJobList");
                var task = Svc.Framework.RunOnFrameworkThread(() => {
                    Callback.Fire(supportJobListAddon, true, 0, classId);
                });
                task.WaitSafely();
            }
        }
    }

    public bool ResetBardBuff()
    {
        unsafe
        {
            // Switch phantom job to bard
            // Then do the job buff action
            // Check that the buff is running with more than 29 minutes left
            // Loop if that's not the case

            // Switch job to Bard
            var jobListAddon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MKDSupportJobList");
            if (jobListAddon->IsVisible)
            {
                Svc.Framework.RunOnFrameworkThread(() => {
                    if (GenericHelpers.IsAddonReady(jobListAddon))
                        Callback.Fire(jobListAddon, true, 0, 6);
                    else
                        Svc.Log.Debug("Support REAL LIST job not ready");
                });
            }

            // Open support Job
            var supportJobAddon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MKDSupportJob");
            var baseSupportGui = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MKDInfo");
            Svc.Log.Debug("isSupportJobVisible?", supportJobAddon->IsVisible.ToString());
            if (!supportJobAddon->IsVisible)
            {
                Svc.Framework.RunOnFrameworkThread(() => {
                    if (GenericHelpers.IsAddonReady(supportJobAddon))
                        Callback.Fire(baseSupportGui, true, 1, 0);
                    else
                        Svc.Log.Debug("Support job not ready");
                });
            }
            else
            {
                Svc.Log.Debug("Support job already visible");
            }

            // Open support job list
            var supportJobListAddon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MKDSupportJob");
            Svc.Log.Debug("isSupportJobListVisible?", supportJobListAddon->IsVisible.ToString());
            if (supportJobListAddon->IsVisible)
            {
                Svc.Framework.RunOnFrameworkThread(() => {
                    if (GenericHelpers.IsAddonReady(supportJobListAddon))
                        Callback.Fire(supportJobListAddon, true, 0, 0, 0);
                    else
                        Svc.Log.Debug("Support job list not ready");
                });
            }
            else
            {
                Svc.Log.Debug("Support job list already visible");
            }

            return false;
        }
    }

    public bool IsBard()
    {
        return HasBuff((uint)Job.Bard);
    }

    public bool IsMonk()
    {
        return HasBuff((uint)Job.Monk);
    }

    public bool IsPaladin()
    {
        return HasBuff((uint)Job.Paladin);
    }

    public bool HasBardBuff()
    {
        return HasBuff((uint)CrescentBuff.Bard);
    }

    public bool HasMonkBuff()
    {
        return HasBuff((uint)CrescentBuff.Monk);
    }

    public bool HadPaladinBuff()
    {
        return HasBuff((uint)CrescentBuff.Paladin);
    }

    private bool HasBuff(uint buffId)
    {
        var currentStatusList = Svc.ClientState.LocalPlayer!.StatusList;
        return currentStatusList.Any(status => status.StatusId == buffId);
    }
}
