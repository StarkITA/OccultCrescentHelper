using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
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

    private bool ExpRecieved = true;

    private float WaitedForExp = 0f;

    private float MaxExpWait = 4f;

    private Plugin plugin;

    public JobManager(Plugin plugin)
    {
        this.plugin = plugin;
    }

    public void Tick(IFramework framework)
    {
        if (!Helpers.IsInOccultCrescent())
        {
            return;
        }

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
            ExpRecieved = false;
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

            var ExpJob = Jobs.FirstOrDefault(job => job.RowId == plugin.config.ExpJob);
            ChangeJob(ExpJob);

            return;
        }

        if (!Svc.Condition[ConditionFlag.InCombat] && state == JobManagerState.InFate)
        {
            state = JobManagerState.PostContent;

            var ExpJob = Jobs.FirstOrDefault(job => job.RowId == plugin.config.ExpJob);
            ChangeJob(ExpJob);

            return;
        }

        if (!Svc.Condition[ConditionFlag.InCombat] && state == JobManagerState.InCriticalEncounter)
        {
            state = JobManagerState.PostExp;
        }

        if (state == JobManagerState.PostContent)
        {
            WaitedForExp += framework.UpdateDelta.Milliseconds / 1000f;

            if (ExpRecieved || WaitedForExp >= MaxExpWait)
            {
                ExpRecieved = false;
                WaitedForExp = 0f;
                state = JobManagerState.PostExp;
                return;
            }
        }
    }

    public void OnChatMessage(
        XivChatType type,
        int timestamp,
        ref SeString sender,
        ref SeString message,
        ref bool isHandled
    )
    {
        if (!Helpers.IsInOccultCrescent())
        {
            return;
        }

        var text = message.ToString();

        var exp_pattern = @"You gain \d+ Phantom .+? experience points\.";
        if (System.Text.RegularExpressions.Regex.IsMatch(text, exp_pattern))
        {
            ExpRecieved = true;
        }

        var ce_pattern = @"You have joined the critical encounter .*?";
        if (
            System.Text.RegularExpressions.Regex.IsMatch(text, ce_pattern)
            && plugin.config.SwitchToExpJobOnCE
            && state == JobManagerState.PreContent
        )
        {
            state = JobManagerState.InCriticalEncounter;

            var Jobs = Svc.Data.GetExcelSheet<MKDSupportJob>().ToList();
            var ExpJob = Jobs.FirstOrDefault(job => job.RowId == plugin.config.ExpJob);

            ChangeJob(ExpJob);
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
