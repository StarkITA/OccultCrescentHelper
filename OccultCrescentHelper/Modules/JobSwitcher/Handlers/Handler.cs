using System.Collections.Generic;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;

namespace OccultCrescentHelper.JobSwitcher.Handlers;

public abstract class Handler : IJobStateHandler
{
    protected readonly JobSwitcher switcher;

    protected readonly List<MKDSupportJob> jobs;

    protected readonly MKDSupportJob expJob;

    protected readonly MKDSupportJob combatJob;

    protected JobSwitcherConfig config
    {
        get => switcher.config;
    }

    public Handler(JobSwitcher switcher, List<MKDSupportJob> jobs, MKDSupportJob expJob, MKDSupportJob combatJob)
    {
        this.switcher = switcher;
        this.jobs = jobs;
        this.expJob = expJob;
        this.combatJob = combatJob;
    }

    public virtual void Enter() { }

    public virtual void Tick(IFramework framework) { }

    public virtual void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled) { }

    protected unsafe void ChangeJob(MKDSupportJob job)
    {
        if (!Helpers.IsInOccultCrescent())
        {
            return;
        }

        byte id = (byte)job.RowId;
        if (PublicContentOccultCrescent.GetInstance()->State.CurrentSupportJob != id)
        {
            PublicContentOccultCrescent.ChangeSupportJob(id);
        }
    }

    protected void ChangeToCombatJob() => ChangeJob(combatJob);

    protected void ChangeToExpJob() => ChangeJob(expJob);

    protected bool IsInCombat() => Svc.Condition[ConditionFlag.InCombat];
}
