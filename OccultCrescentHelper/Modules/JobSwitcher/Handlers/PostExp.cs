using System.Collections.Generic;
using Lumina.Excel.Sheets;

namespace OccultCrescentHelper.JobSwitcher.Handlers;

public class PostExp : Handler
{
    public PostExp(JobSwitcher switcher, List<MKDSupportJob> jobs, MKDSupportJob expJob, MKDSupportJob combatJob)
        : base(switcher, jobs, expJob, combatJob) { }

    public override void Enter()
    {
        switcher.SetState(JobSwitcherState.PreContent);
    }
}
