using System.Collections.Generic;
using Lumina.Excel.Sheets;

namespace OccultCrescentHelper.JobSwitcher.Handlers;

public class PostExp : Handler
{
    public PostExp(JobSwitcher switcher, List<MKDSupportJob> jobs)
        : base(switcher, jobs) { }

    public override void Enter()
    {
        switcher.SetState(JobSwitcherState.PreContent);
    }
}
