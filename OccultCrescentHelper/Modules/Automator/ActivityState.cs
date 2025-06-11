namespace OccultCrescentHelper.Modules.Automator;

public enum ActivityState
{
    Idle,
    Pathfinding,
    WaitingToStartCriticalEncoutner,
    Participating,
    Done,
}

public static class ActivityStateExtensions
{
    public static string ToLabel(this ActivityState state)
    {
        return state switch {
            ActivityState.Idle => "Idle",
            ActivityState.Pathfinding => "Pathfinding",
            ActivityState.WaitingToStartCriticalEncoutner => "Waiting to Start (CE)",
            ActivityState.Participating => "Participating",
            ActivityState.Done => "Done",
            _ => "Unknown",
        };
    }
}

