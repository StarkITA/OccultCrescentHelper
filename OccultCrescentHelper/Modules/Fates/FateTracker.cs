using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using OccultCrescentHelper.Enums;

namespace OccultCrescentHelper.Fates;

public class FateTracker
{
    public Dictionary<uint, IFate> fates = [];

    public Dictionary<uint, FateProgress> tracker { get; } = [];

    public static readonly Dictionary<uint, FateData> data = new Dictionary<uint, FateData>
    {
        {
            1968,
            new FateData
            {
                id = 1968,
                Name = "A Delicate Balance",
                demiatma = Enums.Demiatma.Verdigris,
            }
        },
        {
            1970,
            new FateData
            {
                id = 1970,
                Name = "A Prying Eye",
                demiatma = Demiatma.Azurite,
            }
        },
        {
            1966,
            new FateData
            {
                id = 1966,
                Name = "An Unending Duty",
                demiatma = Demiatma.Malachite,
            }
        },
        {
            1967,
            new FateData
            {
                id = 1967,
                Name = "Brain Drain",
                demiatma = Demiatma.Realgar,
            }
        },
        {
            1971,
            new FateData
            {
                id = 1971,
                Name = "Fatal Allure",
                demiatma = Demiatma.Orpiment,
            }
        },
        {
            1964,
            new FateData
            {
                id = 1964,
                Name = "King of the Crescent",
                demiatma = Demiatma.Orpiment,
            }
        },
        {
            1976,
            new FateData
            {
                id = 1976,
                Name = "Persistent Pots",
                demiatma = Demiatma.Orpiment,
                notes = MonsterNote.PersistentPots,
            }
        },
        {
            1977,
            new FateData
            {
                id = 1977,
                Name = "Pleading Pots",
                demiatma = Demiatma.Verdigris,
                notes = MonsterNote.PersistentPots,
            }
        },
        {
            1962,
            new FateData
            {
                id = 1962,
                Name = "Rough Waters",
                demiatma = Demiatma.Azurite,
            }
        },
        {
            1972,
            new FateData
            {
                id = 1972,
                Name = "Serving Darkness",
                demiatma = Demiatma.CaputMortuum,
            }
        },
        {
            1969,
            new FateData
            {
                id = 1969,
                Name = "Sworn to Soil",
                demiatma = Demiatma.Verdigris,
            }
        },
        {
            1963,
            new FateData
            {
                id = 1963,
                Name = "The Golden Guardian",
                demiatma = Demiatma.Azurite,
            }
        },
        {
            1965,
            new FateData
            {
                id = 1965,
                Name = "The Winged Terror",
                demiatma = Demiatma.Realgar,
            }
        },
    };

    public void Tick(IFramework _)
    {
        var pos = Svc.ClientState.LocalPlayer!.Position;

        fates = Svc.Fates.OrderBy(f => Vector3.Distance(f.Position, pos)).ToDictionary(f => (uint)f.FateId, f => f);

        foreach (var fate in fates.Values)
        {
            if (fate.Progress == 0)
            {
                continue;
            }

            if (!tracker.TryGetValue(fate.FateId, out var progress))
            {
                progress = new FateProgress(fate.FateId);
                tracker[fate.FateId] = progress;
            }

            if (progress.samples.Count == 0 || progress.samples[^1].Progress != fate.Progress)
            {
                progress.AddProgress(fate.Progress);
            }

            if (fate.Progress == 100)
            {
                tracker.Remove(fate.FateId);
            }
        }

        // Remove non-active fates
        var active = fates.Select(f => f.Key).ToHashSet();
        var obsolete = tracker.Keys.Where(id => !active.Contains(id)).ToList();
        foreach (var id in obsolete)
        {
            tracker.Remove(id);
        }
    }
}
