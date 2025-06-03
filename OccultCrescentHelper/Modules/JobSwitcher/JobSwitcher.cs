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

        states = new()
        {
            [JobSwitcherState.PreContent] = new PreContent(this, jobs),
            [JobSwitcherState.InCombat] = new InCombat(this, jobs),
            [JobSwitcherState.InFate] = new InFate(this, jobs),
            [JobSwitcherState.InCriticalEncounter] = new InCriticalEncounter(this, jobs),
            [JobSwitcherState.OccultReturn] = new OccultReturn(this, jobs),
            [JobSwitcherState.PostContent] = new PostContent(this, jobs),
            [JobSwitcherState.PostExp] = new PostExp(this, jobs),
        };

        Svc.Framework.RunOnFrameworkThread(() => SetState(JobSwitcherState.PostExp));
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

    public string GetStateText() => currentState?.ToString() ?? "Unknown State";
}
