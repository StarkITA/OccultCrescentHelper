using System;
using System.Text.RegularExpressions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace OccultCrescentHelper.Modules.Exp;

public class ExpTracker
{
    private float exp = 0f;

    private DateTime startTime = DateTime.UtcNow;

    public ExpTracker()
    {
        Reset();
    }

    public void OnTerritoryChange(ushort _) => Reset();

    public void OnChatMessage(XivChatType type, int timestamp, SeString sender, SeString message, bool isHandled)
    {
        var pattern = @"You gain (\d+) Phantom .+? experience points\.";
        var match = Regex.Match(message.ToString(), pattern);
        if (match.Success)
        {
            exp += int.Parse(match.Groups[1].Value);
        }
    }

    public void Reset()
    {
        exp = 0f;
        startTime = DateTime.UtcNow;
    }

    public float GetExpPerHour()
    {
        var elapsed = (float)(DateTime.UtcNow - startTime).TotalHours;
        if (elapsed <= 0)
            return 0;

        return (exp) / elapsed;
    }
}
