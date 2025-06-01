using System;
using System.Collections.Generic;
using ECommons.DalamudServices;

namespace OccultCrescentHelper.Trackers;

public struct Tracked
{
    public string label { get; } = "";

    public double value = 0;

    private readonly TimeSpan window = TimeSpan.FromHours(1);

    private struct Snapshot
    {
        public DateTime Timestamp;
        public double Value;
    }

    private readonly Queue<Snapshot> history = new Queue<Snapshot>();

    public Tracked(string label, double value)
    {
        this.label = label;
        this.value = value;

        history.Enqueue(new Snapshot { Timestamp = DateTime.UtcNow, Value = value });
    }

    public void UpdateValue(double currentValue)
    {
        if (currentValue == value && history.Count == 1)
        {
            return;
        }

        if (currentValue != value)
        {
            Svc.Log.Debug($"Value updated for '{label}': {value} -> {currentValue}");
        }

        value = currentValue;

        var now = DateTime.UtcNow;
        history.Enqueue(new Snapshot { Timestamp = now, Value = currentValue });

        // Remove any snapshots older than 1 hour
        while (history.Count > 0 && now - history.Peek().Timestamp > window)
        {
            history.Dequeue();
        }
    }

    public double GetRatePerHour()
    {
        if (history.Count < 2)
            return 0;

        var oldest = history.Peek();
        var newest = history.ToArray()[^1];

        var deltaValue = newest.Value - oldest.Value;
        var deltaTime = (newest.Timestamp - oldest.Timestamp).TotalHours;

        if (deltaTime <= 0)
            return 0;

        return deltaValue / deltaTime;
    }

    public void Reset(double value)
    {
        history.Clear();
        this.value = value;
        history.Enqueue(new Snapshot { Timestamp = DateTime.UtcNow, Value = value });
    }
}
