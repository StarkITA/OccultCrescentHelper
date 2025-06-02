using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace OccultCrescentHelper.JobSwitcher.Handlers;

public class PostContent : Handler
{
    private bool expRecieved = false;

    private float waited = 0f;

    private float maxWait = 5f;

    public PostContent(JobSwitcher switcher, List<MKDSupportJob> jobs, MKDSupportJob expJob, MKDSupportJob combatJob)
        : base(switcher, jobs, expJob, combatJob) { }

    public override void Enter()
    {
        ChangeToExpJob();
        expRecieved = false;
        waited = 0f;
    }

    public override void Tick(IFramework framework)
    {
        waited += framework.UpdateDelta.Milliseconds / 1000f;

        if (expRecieved || waited >= maxWait)
        {
            switcher.SetState(JobSwitcherState.PostExp);
        }
    }

    public override void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        var pattern = @"You gain \d+ Phantom .+? experience points\.";
        if (Regex.IsMatch(message.ToString(), pattern))
        {
            expRecieved = true;
        }
    }
}
