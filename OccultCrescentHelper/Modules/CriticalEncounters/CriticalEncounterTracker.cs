using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using OccultCrescentHelper.Data;

namespace OccultCrescentHelper.Modules.CriticalEncounters;

public class CriticalEncounterTracker
{
    public List<DynamicEvent> criticalEncounters = [];

    public Dictionary<uint, EventProgress> progress { get; } = [];

    public unsafe void Tick(IFramework _)
    {
        var pos = Svc.ClientState.LocalPlayer?.Position ?? Vector3.Zero;

        criticalEncounters = PublicContentOccultCrescent.GetInstance()->DynamicEventContainer.Events.ToArray().ToList();

        uint index = 0;
        foreach (var ev in criticalEncounters)
        {
            if (ev.State == DynamicEventState.Battle)
            {
                if (ev.Progress == 0)
                {
                    continue;
                }


                if (!this.progress.TryGetValue(index, out var progress))
                {
                    progress = new EventProgress();
                    this.progress[index] = progress;
                }

                if (progress.samples.Count == 0 || progress.samples[^1].Progress != ev.Progress)
                {
                    progress.AddProgress(ev.Progress);
                }

                if (ev.Progress == 100)
                {
                    this.progress.Remove(index);
                }
            }
            else
            {
                this.progress.Remove(index);
            }

            index++;
        }
    }

}
