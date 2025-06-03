using System.Collections.Generic;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using OccultCrescentHelper.Enums;

namespace OccultCrescentHelper.CriticalEncounters;

public unsafe struct DynamicEventContainerView
{
    private DynamicEvent* events;

    public DynamicEventContainerView(DynamicEventContainer* container)
    {
        events = (DynamicEvent*)((byte*)container + 0x08); // offset of _events
    }

    public ref DynamicEvent this[int index] => ref events[index];

    public int Count => 16;
}

public class CriticalEncounterTracker
{
    public List<DynamicEvent> criticalEncounters = [];

    public Dictionary<uint, CriticalEncounterProgress> tracker { get; } = [];

    public static readonly Dictionary<uint, CriticalEncounterData> data = new Dictionary<uint, CriticalEncounterData>
    {
        {
            0,
            new CriticalEncounterData
            {
                id = 0,
                Name = "The Forked Tower: Blood",
                // Fill in data as needed
            }
        },
        {
            1,
            new CriticalEncounterData
            {
                id = 1,
                Name = "Scourge of the Mind",
                demiatma = Demiatma.Azurite,
                monster = Monster.MysteriousMindflayer,
            }
        },
        {
            2,
            new CriticalEncounterData
            {
                id = 2,
                Name = "The Black Regiment",
                demiatma = Demiatma.Orpiment,
                soulshard = SoulShard.Ranger,
                monster = Monster.BlackStar,
                notes = MonsterNote.BlackChocobos,
            }
        },
        {
            3,
            new CriticalEncounterData
            {
                id = 3,
                Name = "The Unbridled",
                demiatma = Demiatma.Azurite,
                soulshard = SoulShard.Berserker,
                monster = Monster.CrescentBerserker,
                notes = MonsterNote.CrescentBerserker,
            }
        },
        {
            4,
            new CriticalEncounterData
            {
                id = 4,
                Name = "Crawling Death",
                demiatma = Demiatma.Azurite,
                monster = Monster.DeathClawOccultCrescent,
            }
        },
        {
            5,
            new CriticalEncounterData
            {
                id = 5,
                Name = "Calamity Bound",
                demiatma = Demiatma.Verdigris,
                monster = Monster.CloisterDemon,
                notes = MonsterNote.CloisterDemon,
            }
        },
        {
            6,
            new CriticalEncounterData
            {
                id = 6,
                Name = "Trial by Claw",
                demiatma = Demiatma.Malachite,
                monster = Monster.CrystalDragon,
            }
        },
        {
            7,
            new CriticalEncounterData
            {
                id = 7,
                Name = "From Times Bygone",
                demiatma = Demiatma.Malachite,
                monster = Monster.MythicIdol,
                notes = MonsterNote.MythicIdol,
            }
        },
        {
            8,
            new CriticalEncounterData
            {
                id = 8,
                Name = "Company of Stone",
                demiatma = Demiatma.CaputMortuum,
                monster = Monster.OccultKnight,
            }
        },
        {
            9,
            new CriticalEncounterData
            {
                id = 9,
                Name = "Shark Attack",
                demiatma = Demiatma.Realgar,
                monster = Monster.NymianPotaladus,
                notes = MonsterNote.NymianPotaladus,
            }
        },
        {
            10,
            new CriticalEncounterData
            {
                id = 10,
                Name = "On the Hunt",
                demiatma = Demiatma.CaputMortuum,
                soulshard = SoulShard.Oracle,
                monster = Monster.LionRampant,
            }
        },
        {
            11,
            new CriticalEncounterData
            {
                id = 11,
                Name = "With Extreme Prejudice",
                demiatma = Demiatma.Realgar,
                monster = Monster.CommandUm,
            }
        },
        {
            12,
            new CriticalEncounterData
            {
                id = 12,
                Name = "Noise Complaint",
                demiatma = Demiatma.Orpiment,
                monster = Monster.NeoGarula,
            }
        },
        {
            13,
            new CriticalEncounterData
            {
                id = 13,
                Name = "Cursed Concern",
                demiatma = Demiatma.Realgar,
                monster = Monster.TradeTortoise,
                notes = MonsterNote.TradeTortoise,
            }
        },
        {
            14,
            new CriticalEncounterData
            {
                id = 14,
                Name = "Eternal Watch",
                demiatma = Demiatma.CaputMortuum,
                monster = Monster.RepairedLion,
            }
        },
        {
            15,
            new CriticalEncounterData
            {
                id = 15,
                Name = "Flame of Dusk",
                demiatma = Demiatma.Malachite,
                monster = Monster.Hinkypunk,
            }
        },
    };

    public unsafe void Tick(IFramework _)
    {
        var instance = PublicContentOccultCrescent.GetInstance();
        if (instance == null)
        {
            return;
        }

        var view = new DynamicEventContainerView(&instance->DynamicEventContainer);

        criticalEncounters.Clear();
        for (uint i = 0; i < view.Count; i++)
        {
            var ce = view[(int)i];
            criticalEncounters.Add(ce);

            if (ce.State == DynamicEventState.Battle)
            {
                if (ce.Progress == 0)
                {
                    continue;
                }

                if (!tracker.TryGetValue(i, out var progress))
                {
                    progress = new CriticalEncounterProgress(i);
                    tracker[i] = progress;
                }

                if (progress.samples.Count == 0 || progress.samples[^1].Progress != ce.Progress)
                {
                    progress.AddProgress(ce.Progress);
                }

                if (ce.Progress == 100)
                {
                    tracker.Remove(i);
                }
            }
            else
            {
                tracker.Remove(i);
            }
        }
    }
}
