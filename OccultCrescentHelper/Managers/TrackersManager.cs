using System.Collections.Generic;
using Dalamud.Plugin.Services;
using OccultCrescentHelper.Trackers;

namespace OccultCrescentHelper.Managers;

public class TrackersManager
{
    private static TrackersManager _instance;
    private static readonly object _lock = new object();

    private List<ITracker> trackers = [];

    private float ElapsedTime = 0f;

    private const float Interval = 1f;

    // Private constructor to prevent instantiation
    private TrackersManager() { }

    // Public property to access the singleton instance
    public static TrackersManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new TrackersManager();
                    _instance.InitializeTrackers();
                }
            }

            return _instance;
        }
    }

    private void InitializeTrackers()
    {
        trackers.Add(new CurrencyTracker());
    }

    public void Tick(IFramework framework)
    {
        float SecondsSinceLastTick = framework.UpdateDelta.Milliseconds / 1000f;
        ElapsedTime += SecondsSinceLastTick;
        if (ElapsedTime < Interval)
        {
            return;
        }

        ElapsedTime -= Interval;

        foreach (var tracker in trackers)
        {
            tracker.Tick();
        }
    }

    public List<Tracked> GetData()
    {
        var tracked = new List<Tracked>();
        foreach (var tracker in trackers)
        {
            tracked.AddRange(tracker.GetData());
        }

        return tracked;
    }
}
