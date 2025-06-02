using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using Lumina.Excel.Sheets;
using OccultCrescentHelper.JobSwitcher.Handlers;

namespace OccultCrescentHelper.JobSwitcher;

public class JobSwitcher
{
    public readonly Plugin plugin;

    private IJobStateHandler currentState;

    private Dictionary<JobSwitcherState, IJobStateHandler> states;

    public JobSwitcherConfig config
    {
        get => plugin.config.JobSwitcherConfig;
    }

    public JobSwitcher(Plugin plugin)
    {
        this.plugin = plugin;

        var jobs = Svc.Data.GetExcelSheet<MKDSupportJob>().ToList();
        var expJob = jobs.FirstOrDefault(job => job.RowId == plugin.config.JobSwitcherConfig.ExpJob);
        var combatJob = jobs.FirstOrDefault(job => job.RowId == plugin.config.JobSwitcherConfig.CombatJob);

        states = new()
        {
            [JobSwitcherState.PreContent] = new PreContent(this, jobs, expJob, combatJob),
            [JobSwitcherState.InCombat] = new InCombat(this, jobs, expJob, combatJob),
            [JobSwitcherState.InFate] = new InFate(this, jobs, expJob, combatJob),
            [JobSwitcherState.InCriticalEncounter] = new InCriticalEncounter(this, jobs, expJob, combatJob),
            [JobSwitcherState.OccultReturn] = new OccultReturn(this, jobs, expJob, combatJob),
            [JobSwitcherState.PostContent] = new PostContent(this, jobs, expJob, combatJob),
            [JobSwitcherState.PostExp] = new PostExp(this, jobs, expJob, combatJob),
        };

        SetState(JobSwitcherState.PostExp);
    }

    public void Tick(IFramework framework)
    {
        if (!plugin.config.JobSwitcherConfig.SwitchJobOnCombatEnd)
            return;

        currentState.Tick(framework);
    }

    public void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!Helpers.IsInOccultCrescent())
        {
            return;
        }

        currentState.OnChatMessage(type, timestamp, ref sender, ref message, ref isHandled);
    }

    public void SetState(JobSwitcherState newState)
    {
        currentState = states[newState];
        currentState.Enter();
    }

    public string GetStateText() => currentState.ToString() ?? "Unknown State";
}
