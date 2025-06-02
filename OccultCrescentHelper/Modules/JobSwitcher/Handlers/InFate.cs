using System.Collections.Generic;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace OccultCrescentHelper.JobSwitcher.Handlers;

public class InFate : Handler
{
    public InFate(JobSwitcher switcher, List<MKDSupportJob> jobs, MKDSupportJob expJob, MKDSupportJob combatJob)
        : base(switcher, jobs, expJob, combatJob) { }

    public override void Tick(IFramework _)
    {
        if (IsInCombat())
        {
            return;
        }

        if (!config.ReturnAfterFate)
        {
            switcher.SetState(JobSwitcherState.PostContent);
            return;
        }

        // ActionManager.Instance()->UseAction(ActionType.GeneralAction, 8);
    }
}
