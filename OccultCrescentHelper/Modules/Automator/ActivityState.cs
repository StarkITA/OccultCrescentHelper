namespace OccultCrescentHelper.Modules.Automator;

public enum ActivityState
{
    Idle,
    Pathfinding,

    // Fate specific
    WaitingForFateTarget,

    // Ce specific
    WaitingToStartCriticalEncoutner,


    Participating,
}

public static class ActivityStateExtensions
{
    public static string ToLabel(this ActivityState state)
    {
        return state switch {
            ActivityState.Idle => "Idle",
            ActivityState.Pathfinding => "Pathfinding",
            ActivityState.WaitingForFateTarget => "Waiting for Target (Fate)",
            ActivityState.WaitingToStartCriticalEncoutner => "Waiting to Start (CE)",
            ActivityState.Participating => "Participating",
            _ => "Unknown",
        };
    }
}

