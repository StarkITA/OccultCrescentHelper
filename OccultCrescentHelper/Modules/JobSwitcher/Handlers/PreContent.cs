using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using Lumina.Excel.Sheets;

namespace OccultCrescentHelper.JobSwitcher.Handlers;

public class PreContent : Handler
{
    public PreContent(JobSwitcher switcher, List<MKDSupportJob> jobs)
        : base(switcher, jobs) { }

    public override void Enter() => ChangeToCombatJob();

    public override void Tick(IFramework _)
    {
        if (!IsInCombat())
        {
            return;
        }

        if (IsInFate())
        {
            switcher.SetState(JobSwitcherState.InFate);
        }
        else
        {
            switcher.SetState(JobSwitcherState.InCombat);
        }
    }

    public override void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        var pattern = @"You have joined the critical encounter .*?";
        if (Regex.IsMatch(message.ToString(), pattern))
        {
            switcher.SetState(JobSwitcherState.InCriticalEncounter);
        }
    }

    private unsafe bool IsInFate() => FateManager.Instance()->CurrentFate is not null;
}
