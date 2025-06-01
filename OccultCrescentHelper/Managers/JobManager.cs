using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;

namespace OccultCrescentHelper.Managers;

public unsafe class JobManager
{
    private enum JobManagerState
    {
        PreContent,
        InCombat,
        InFate,
        InCriticalEncounter,
        PostContent,
        PostExp,
    }

    private JobManagerState state = JobManagerState.PostExp;

    private float PostContentTime = 0.0f;

    private float PostContentWait = 4.0f;
    private Plugin plugin;

    public JobManager(Plugin plugin)
    {
        this.plugin = plugin;
    }

    public void Tick(IFramework framework)
    {
        if (!plugin.config.SwitchJobOnCombatEnd)
        {
            return;
        }

        var Jobs = Svc.Data.GetExcelSheet<MKDSupportJob>().ToList();

        if (state == JobManagerState.PostExp)
        {
            var CombatJob = Jobs.FirstOrDefault(job => job.RowId == plugin.config.CombatJob);
            ChangeJob(CombatJob);
            state = JobManagerState.PreContent;
            return;
        }

        if (Svc.Condition[ConditionFlag.InCombat] && state == JobManagerState.PreContent)
        {
            if (FateManager.Instance()->CurrentFate is not null)
            {
                state = JobManagerState.InFate;
            }
            else
            {
                state = JobManagerState.InCombat;
            }

            return;
        }

        if (!Svc.Condition[ConditionFlag.InCombat] && state == JobManagerState.InCombat)
        {
            state = JobManagerState.PostContent;
            PostContentTime = 0.0f;

            var ExpJob = Jobs.FirstOrDefault(job => job.RowId == plugin.config.ExpJob);
            ChangeJob(ExpJob);

            return;
        }

        if (state == JobManagerState.PostContent)
        {
            PostContentTime += framework.UpdateDelta.Milliseconds / 1000f;

            if (PostContentTime >= PostContentWait)
            {
                state = JobManagerState.PostExp;
                return;
            }
        }
    }

    public void ChangeJob(MKDSupportJob job)
    {
        Agent.SendEvent(AgentId.MKDSupportJobList, 1, 0, job.RowId);
    }

    public string GetStateText()
    {
        return state.ToString();
    }
}
