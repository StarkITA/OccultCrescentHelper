using System.Collections.Generic;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace OccultCrescentHelper.JobSwitcher.Handlers;

public class InCriticalEncounter : Handler
{
    public InCriticalEncounter(JobSwitcher switcher, List<MKDSupportJob> jobs)
        : base(switcher, jobs) { }

    public override void Enter()
    {
        if (!switcher.config.SwitchToExpJobOnCE)
        {
            return;
        }

        ChangeToExpJob();
    }

    public override void Tick(IFramework _)
    {
        if (IsInCombat())
        {
            return;
        }

        if (!config.ReturnAfterCE)
        {
            switcher.SetState(JobSwitcherState.PostExp);
            return;
        }

        switcher.SetState(JobSwitcherState.OccultReturn);
    }
}
