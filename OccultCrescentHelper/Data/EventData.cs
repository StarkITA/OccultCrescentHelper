using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using OccultCrescentHelper.Data.Paths;
using OccultCrescentHelper.Enums;
using Ocelot.Prowler;

namespace OccultCrescentHelper.Data;

public struct EventData
{
    public uint id;

    public EventType type;

    public string Name;

    public Demiatma? demiatma;

    public SoulShard? soulshard;

    public MonsterNote? notes;

    public Monster? monster;

    public Aethernet? aethernet;

    public Vector3? start;

    public Func<Func<List<IProwlerAction>>> pathFactory;

    public static readonly Dictionary<uint, EventData> Fates = new Dictionary<uint, EventData>
    {
        {
            1962,
            new EventData
            {
                id = 1962,
                type = EventType.Fate,
                Name = "Rough Waters",
                demiatma = Demiatma.Azurite,
                // pathFactory = RoughWaters.GetPath,
                start = new(162.00f, 56.00f, 676.00f),
            }
        },
        {
            1963,
            new EventData
            {
                id = 1963,
                type = EventType.Fate,
                Name = "The Golden Guardian",
                demiatma = Demiatma.Azurite,
                // pathFactory = TheGoldenGuardian.GetPath,
                start = new(373.20f, 70.00f, 486.00f),
            }
        },
        {
            1964,
            new EventData
            {
                id = 1964,
                type = EventType.Fate,
                Name = "King of the Crescent",
                demiatma = Demiatma.Orpiment,
                start = new(-226.10f, 116.38f, 254.00f),
            }
        },
        {
            1965,
            new EventData
            {
                id = 1965,
                type = EventType.Fate,
                Name = "The Winged Terror",
                demiatma = Demiatma.Realgar,
                aethernet = Aethernet.TheWanderersHaven,
                // pathFactory = TheWingedTerror.GetPath,
                start = new(-548.50f, 3.00f, -595.00f),
            }
        },
        {
            1966,
            new EventData
            {
                id = 1966,
                type = EventType.Fate,
                Name = "An Unending Duty",
                demiatma = Demiatma.Malachite,
                // pathFactory = AnUnendingDuty.GetPath,
                start = new(-223.10f, 107.00f, 36.00f),
            }
        },
        {
            1967,
            new EventData
            {
                id = 1967,
                type = EventType.Fate,
                Name = "Brain Drain",
                demiatma = Demiatma.Realgar,
                aethernet = Aethernet.CrystallizedCaverns,
                start = new(-48.10f, 111.76f, -320.00f),
            }
        },
        {
            1968,
            new EventData
            {
                id = 1968,
                type = EventType.Fate,
                Name = "A Delicate Balance",
                demiatma = Demiatma.Verdigris,
                start = new(-370.00f, 75.00f, 650.00f),
            }
        },
        {
            1969,
            new EventData
            {
                id = 1969,
                type = EventType.Fate,
                Name = "Sworn to Soil",
                demiatma = Demiatma.Verdigris,
                start = new(-589.10f, 96.50f, 333.00f),
            }
        },
        {
            1970,
            new EventData
            {
                id = 1970,
                type = EventType.Fate,
                Name = "A Prying Eye",
                demiatma = Demiatma.Azurite,
                // pathFactory = APryingEye.GetPath,
                start = new(-71.00f, 71.31f, 557.00f),
            }
        },
        {
            1971,
            new EventData
            {
                id = 1971,
                type = EventType.Fate,
                Name = "Fatal Allure",
                demiatma = Demiatma.Orpiment,
                start = new(79.00f, 97.86f, 278.00f),
            }
        },
        {
            1972,
            new EventData
            {
                id = 1972,
                type = EventType.Fate,
                Name = "Serving Darkness",
                demiatma = Demiatma.CaputMortuum,
                start = new(413.00f, 96.00f, -13.00f),
            }
        },
        {
            1976,
            new EventData
            {
                id = 1976,
                type = EventType.Fate,
                Name = "Persistent Pots",
                demiatma = Demiatma.Orpiment,
                notes = MonsterNote.PersistentPots,
                start = new(200.00f, 111.73f, -215.00f),
            }
        },
        {
            1977,
            new EventData
            {
                id = 1977,
                type = EventType.Fate,
                Name = "Pleading Pots",
                demiatma = Demiatma.Verdigris,
                notes = MonsterNote.PersistentPots,
                start = new(-481.00f, 75.00f, 528.00f),
            }
        },
    };

    public static readonly Dictionary<uint, EventData> CriticalEncounters = new Dictionary<uint, EventData>
    {
        {
            0,
            new EventData
            {
                id = 0,
                type = EventType.CriticalEncounter,
                Name = "The Forked Tower: Blood",
            }
        },
        {
            1,
            new EventData
            {
                id = 1,
                type = EventType.CriticalEncounter,
                Name = "Scourge of the Mind",
                demiatma = Demiatma.Azurite,
                monster = Monster.MysteriousMindflayer,
                aethernet = Aethernet.Eldergrowth,
                // pathFactory = ScourgeOfTheMind.GetPath
            }
        },
        {
            2,
            new EventData
            {
                id = 2,
                type = EventType.CriticalEncounter,
                Name = "The Black Regiment",
                demiatma = Demiatma.Orpiment,
                soulshard = SoulShard.Ranger,
                monster = Monster.BlackStar,
                notes = MonsterNote.BlackChocobos,
                aethernet = Aethernet.Eldergrowth,
                // pathFactory = TheBlackRegiment.GetPath
            }
        },
        {
            3,
            new EventData
            {
                id = 3,
                type = EventType.CriticalEncounter,
                Name = "The Unbridled",
                demiatma = Demiatma.Azurite,
                soulshard = SoulShard.Berserker,
                monster = Monster.CrescentBerserker,
                notes = MonsterNote.CrescentBerserker,
                aethernet = Aethernet.Eldergrowth,
                // pathFactory = TheUnbridled.GetPath
            }
        },
        {
            4,
            new EventData
            {
                id = 4,
                type = EventType.CriticalEncounter,
                Name = "Crawling Death",
                demiatma = Demiatma.Azurite,
                monster = Monster.DeathClawOccultCrescent,
                aethernet = Aethernet.Eldergrowth,
                // pathFactory = CrawlingDeath.GetPath
            }
        },
        {
            5,
            new EventData
            {
                id = 5,
                type = EventType.CriticalEncounter,
                Name = "Calamity Bound",
                demiatma = Demiatma.Verdigris,
                monster = Monster.CloisterDemon,
                notes = MonsterNote.CloisterDemon,
                aethernet = Aethernet.Stonemarsh,
            }
        },
        {
            6,
            new EventData
            {
                id = 6,
                type = EventType.CriticalEncounter,
                Name = "Trial by Claw",
                demiatma = Demiatma.Malachite,
                monster = Monster.CrystalDragon,
                aethernet = Aethernet.CrystallizedCaverns,
            }
        },
        {
            7,
            new EventData
            {
                id = 7,
                type = EventType.CriticalEncounter,
                Name = "From Times Bygone",
                demiatma = Demiatma.Malachite,
                monster = Monster.MythicIdol,
                notes = MonsterNote.MythicIdol,
                aethernet = Aethernet.Stonemarsh,
                // pathFactory = FromTimesBygone.GetPath
            }
        },
        {
            8,
            new EventData
            {
                id = 8,
                type = EventType.CriticalEncounter,
                Name = "Company of Stone",
                demiatma = Demiatma.CaputMortuum,
                monster = Monster.OccultKnight,
                aethernet = Aethernet.BaseCamp,
                // pathFactory = CompanyOfStone.GetPath
            }
        },
        {
            9,
            new EventData
            {
                id = 9,
                type = EventType.CriticalEncounter,
                Name = "Shark Attack",
                demiatma = Demiatma.Realgar,
                monster = Monster.NymianPotaladus,
                notes = MonsterNote.NymianPotaladus,
                aethernet = Aethernet.TheWanderersHaven,
            }
        },
        {
            10,
            new EventData
            {
                id = 10,
                type = EventType.CriticalEncounter,
                Name = "On the Hunt",
                demiatma = Demiatma.CaputMortuum,
                soulshard = SoulShard.Oracle,
                monster = Monster.LionRampant,
                aethernet = Aethernet.Eldergrowth,
            }
        },
        {
            11,
            new EventData
            {
                id = 11,
                type = EventType.CriticalEncounter,
                Name = "With Extreme Prejudice",
                demiatma = Demiatma.Realgar,
                monster = Monster.CommandUm,
                aethernet = Aethernet.TheWanderersHaven,
            }
        },
        {
            12,
            new EventData
            {
                id = 12,
                type = EventType.CriticalEncounter,
                Name = "Noise Complaint",
                demiatma = Demiatma.Orpiment,
                monster = Monster.NeoGarula,
                aethernet = Aethernet.BaseCamp,
            }
        },
        {
            13,
            new EventData
            {
                id = 13,
                type = EventType.CriticalEncounter,
                Name = "Cursed Concern",
                demiatma = Demiatma.Realgar,
                monster = Monster.TradeTortoise,
                notes = MonsterNote.TradeTortoise,
                aethernet = Aethernet.TheWanderersHaven,
            }
        },
        {
            14,
            new EventData
            {
                id = 14,
                type = EventType.CriticalEncounter,
                Name = "Eternal Watch",
                demiatma = Demiatma.CaputMortuum,
                monster = Monster.RepairedLion,
                aethernet = Aethernet.Eldergrowth,
            }
        },
        {
            15,
            new EventData
            {
                id = 15,
                type = EventType.CriticalEncounter,
                Name = "Flame of Dusk",
                demiatma = Demiatma.Malachite,
                monster = Monster.Hinkypunk,
                aethernet = Aethernet.CrystallizedCaverns,
            }
        },
    };
}
