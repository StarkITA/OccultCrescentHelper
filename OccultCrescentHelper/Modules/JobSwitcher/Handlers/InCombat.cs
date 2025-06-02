using System.Collections.Generic;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace OccultCrescentHelper.JobSwitcher.Handlers;

public class InCombat : Handler
{
    public InCombat(JobSwitcher switcher, List<MKDSupportJob> jobs, MKDSupportJob expJob, MKDSupportJob combatJob)
        : base(switcher, jobs, expJob, combatJob) { }

    public override void Tick(IFramework _)
    {
        if (IsInCombat())
        {
            return;
        }

        switcher.SetState(JobSwitcherState.PostContent);
    }
}
