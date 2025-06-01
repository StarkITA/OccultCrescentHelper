using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.Interop;
using OccultCrescentHelper.Enums;

namespace OccultCrescentHelper.Managers;

public struct FateData
{
    public uint id;

    public string Name;

    public Demiatma? demiatma;

    public SoulShard? soulshard;

    public MonsterNote? notes;

    public Monster? monster;
}

public class FateProgressTracker
{
    private const int MaxSamples = 100;

    public uint FateId { get; }

    public List<ProgressSample> ProgressSamples { get; } = new();

    public FateProgressTracker(uint fateId)
    {
        FateId = fateId;
    }

    public void AddProgress(float progress)
    {
        if (ProgressSamples.Count >= MaxSamples)
            ProgressSamples.RemoveAt(0);

        ProgressSamples.Add(new ProgressSample(progress, DateTimeOffset.UtcNow));
    }

    public TimeSpan? EstimateTimeToCompletion()
    {
        if (ProgressSamples.Count < 2)
            return null;

        var first = ProgressSamples.First();
        var last = ProgressSamples.Last();

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

public class FatesManager
{
    public static List<Pointer<FateContext>> fates = [];

    public static Dictionary<uint, FateProgressTracker> FateProgress = new();

    public static readonly Dictionary<uint, FateData> FateData = new Dictionary<uint, FateData>
    {
        {
            1968,
            new FateData
            {
                id = 1968,
                Name = "A Delicate Balance",
                demiatma = Demiatma.Verdigris,
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
        // Critical Encounters
        {
            10001,
            new FateData
            {
                id = 10001,
                Name = "Calamity Bound",
                demiatma = Demiatma.Verdigris,
                monster = Monster.CloisterDemon,
                notes = MonsterNote.CloisterDemon,
            }
        },
        {
            10002,
            new FateData
            {
                id = 10002,
                Name = "Company of Stone",
                demiatma = Demiatma.CaputMortuum,
                monster = Monster.OccultKnight,
            }
        },
        {
            10003,
            new FateData
            {
                id = 10003,
                Name = "Crawling Death",
                demiatma = Demiatma.Azurite,
                monster = Monster.DeathClawOccultCrescent,
            }
        },
        {
            10004,
            new FateData
            {
                id = 10004,
                Name = "Cursed Concern",
                demiatma = Demiatma.Realgar,
                monster = Monster.TradeTortoise,
                notes = MonsterNote.TradeTortoise,
            }
        },
        {
            10005,
            new FateData
            {
                id = 10005,
                Name = "Eternal Watch",
                demiatma = Demiatma.CaputMortuum,
                monster = Monster.RepairedLion,
            }
        },
        {
            10006,
            new FateData
            {
                id = 10006,
                Name = "Flame of Dusk",
                demiatma = Demiatma.Malachite,
                monster = Monster.Hinkypunk,
            }
        },
        {
            10007,
            new FateData
            {
                id = 10007,
                Name = "From Times Bygone",
                demiatma = Demiatma.Malachite,
                monster = Monster.MythicIdol,
                notes = MonsterNote.MythicIdol,
            }
        },
        {
            10008,
            new FateData
            {
                id = 10008,
                Name = "Noise Complaint",
                demiatma = Demiatma.Orpiment,
                monster = Monster.NeoGarula,
            }
        },
        {
            10009,
            new FateData
            {
                id = 10009,
                Name = "On the Hunt",
                demiatma = Demiatma.CaputMortuum,
                soulshard = SoulShard.Oracle,
                monster = Monster.LionRampant,
            }
        },
        {
            10010,
            new FateData
            {
                id = 10010,
                Name = "Scourge of the Mind",
                demiatma = Demiatma.Azurite,
                monster = Monster.MysteriousMindflayer,
            }
        },
        {
            10011,
            new FateData
            {
                id = 10011,
                Name = "Shark Attack",
                demiatma = Demiatma.Realgar,
                monster = Monster.NymianPotaladus,
                notes = MonsterNote.NymianPotaladus,
            }
        },
        {
            10012,
            new FateData
            {
                id = 10012,
                Name = "The Black Regiment",
                demiatma = Demiatma.Orpiment,
                soulshard = SoulShard.Ranger,
                monster = Monster.BlackStar,
                notes = MonsterNote.BlackChocobos,
            }
        },
        {
            10013,
            new FateData
            {
                id = 10013,
                Name = "The Unbridled",
                demiatma = Demiatma.Azurite,
                soulshard = SoulShard.Berserker,
                monster = Monster.CrescentBerserker,
                notes = MonsterNote.CrescentBerserker,
            }
        },
        {
            10014,
            new FateData
            {
                id = 10014,
                Name = "Trial by Claw",
                demiatma = Demiatma.Malachite,
                monster = Monster.CrystalDragon,
            }
        },
        {
            10015,
            new FateData
            {
                id = 10015,
                Name = "With Extreme Prejudice",
                demiatma = Demiatma.Realgar,
                monster = Monster.CommandUm,
            }
        },
    };

    public static unsafe void UpdateFatesList(IFramework framework)
    {
        fates = FateManager
            .Instance()
            ->Fates.AsSpan()
            .ToArray()
            .Where(f => f.Value is not null)
            .OrderBy(f =>
                Vector3.Distance(Svc.ClientState.LocalPlayer!.Position, f.Value->Location)
            )
            .ToList();

        foreach (var f in fates)
        {
            var fateId = f.Value->FateId;
            var progress = f.Value->Progress;
            if (progress == 0)
            {
                continue;
            }

            if (!FateProgress.TryGetValue(fateId, out var tracker))
            {
                tracker = new FateProgressTracker(fateId);
                FateProgress[fateId] = tracker;
            }

            if (
                tracker.ProgressSamples.Count == 0
                || tracker.ProgressSamples[^1].Progress != progress
            )
            {
                tracker.AddProgress(progress);
            }

            if (progress == 100)
            {
                FateProgress.Remove(fateId);
            }
        }

        var activeFateIds = fates.Select(f => f.Value->FateId).ToHashSet();
        var obsoleteFates = FateProgress
            .Keys.Where(id => !activeFateIds.Contains((ushort)id))
            .ToList();

        foreach (var id in obsoleteFates)
        {
            FateProgress.Remove(id);
        }
    }
}
