using System;
using System.Collections.Generic;
using System.Linq;

namespace OccultCrescentHelper.CriticalEncounters;

public class CriticalEncounterProgress
{
    private const int MaxSamples = 100;

    public uint FateId { get; }

    public List<ProgressSample> samples { get; } = new();

    public CriticalEncounterProgress(uint fateId)
    {
        FateId = fateId;
    }

    public void AddProgress(float progress)
    {
        if (samples.Count >= MaxSamples)
            samples.RemoveAt(0);

        samples.Add(new ProgressSample(progress, DateTimeOffset.UtcNow));
    }

    public TimeSpan? EstimateTimeToCompletion()
    {
        if (samples.Count < 2)
            return null;

        var first = samples.First();
        var last = samples.Last();

        float deltaProgress = last.Progress - first.Progress;
        double deltaSeconds = (last.Timestamp - first.Timestamp).TotalSeconds;

        if (deltaProgress <= 0 || deltaSeconds <= 0)
            return null;

        float remainingProgress = 100f - last.Progress;
        double ratePerSecond = deltaProgress / deltaSeconds;
        double estimatedSecondsRemaining = remainingProgress / ratePerSecond;

        return TimeSpan.FromSeconds(estimatedSecondsRemaining);
    }

    public record ProgressSample(float Progress, DateTimeOffset Timestamp);
}
