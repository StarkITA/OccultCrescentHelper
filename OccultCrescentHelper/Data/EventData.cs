using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using OccultCrescentHelper.Enums;

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

    public Vector3? position;

    public List<Vector3> customPath;

    public List<Vector3> GetPath(Vector3 start, Vector3 end)
    {
        var path = customPath.ToList();
        path.Add(end);

        int startIndex = 0;
        float closestDistSq = float.MaxValue;

        for (int i = 0; i < path.Count; i++)
        {
            var toNode = path[i] - start;
            var pathDirection = path[^1] - start;

            if (Vector3.Dot(Vector3.Normalize(pathDirection), Vector3.Normalize(toNode)) > 0)
            {
                float distSq = toNode.LengthSquared();
                if (distSq < closestDistSq)
                {
                    closestDistSq = distSq;
                    startIndex = i;
                }
            }
        }

        return path.Skip(startIndex).ToList();
    }

    public bool happy;

    public static readonly Dictionary<uint, EventData> Fates = new Dictionary<uint, EventData>
    {
        {
            1968,
            new EventData
            {
                id = 1968,
                type = EventType.Fate,
                Name = "A Delicate Balance",
                demiatma = Enums.Demiatma.Verdigris,
                position = new Vector3(370f, 75f, 650f),
                happy = true,
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
                happy = true,
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
                happy = true,
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
                happy = true,
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
                happy = true,
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
                happy = true,
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
                happy = true,
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
                happy = true,
            }
        },
        {
            1962,
            new EventData
            {
                id = 1962,
                type = EventType.Fate,
                Name = "Rough Waters",
                demiatma = Demiatma.Azurite,
                happy = false,
                customPath = [
                    new Vector3(297.55f, 101.82f, 325.07f),
                    new Vector3(263.01f, 78.22f, 421.51f),
                    new Vector3(177.16f, 56.10f, 600.17f),
                    new Vector3(166.44f, 56.00f, 671.85f)
                ]
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
                happy = true,
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
                happy = true,
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
                happy = false,
                customPath = [
                    new Vector3(307.57f, 102.81f, 317.15f),
                    new Vector3(357.11f, 70.24f, 460.74f),
                ]

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
                happy = false,
                customPath = [
                    new Vector3(-238.00f, -0.30f, -595.39f),
                    new Vector3(-394.48f, 6.10f, -590.18f),
                    new Vector3(-445.01f, -0.30f, -596.92f),
                    new Vector3(-460.41f, 2.94f, -590.81f),
                    new Vector3(-462.06f, 3.50f, -591.16f),
                    new Vector3(-509.41f, 3.47f, -604.07f),
                    new Vector3(-517.35f, 3.00f, -605.31f),
                ]
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
                happy = false,
                customPath = [
                    new Vector3(307.11f, 102.79f, 317.29f),
                    new Vector3(354.29f, 70.47f, 462.42f),
                    new Vector3(361.57f, 69.90f, 632.37f),
                    new Vector3(338.65f, 56.00f, 655.20f),
                    new Vector3(355.12f, 56.00f, 691.13f),
                    new Vector3(331.61f, 70.00f, 715.26f),
                ]
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
                happy = false,
                customPath = [
                    new Vector3(314.32f, 102.90f, 303.47f),
                    new Vector3(404.39f, 66.29f, 352.24f),
                ]
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
                happy = false,
                customPath = [
                    new Vector3(315.07f, 102.97f, 308.06f),
                    new Vector3(381.26f, 71.57f, 358.78f),
                    new Vector3(436.64f, 70.00f, 473.35f),
                    new Vector3(450.97f, 70.00f, 494.70f),
                    new Vector3(524.25f, 70.30f, 506.50f),
                    new Vector3(567.54f, 70.00f, 512.61f),
                    new Vector3(584.85f, 70.30f, 567.97f),
                    new Vector3(594.35f, 70.00f, 583.16f),
                    new Vector3(606.60f, 70.30f, 585.62f),
                    new Vector3(610.67f, 70.30f, 588.47f),
                    new Vector3(615.74f, 69.97f, 626.67f),
                    new Vector3(621.52f, 79.00f, 773.71f),
                ]
                // @todo custom path
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
                happy = false,
                customPath = [
                    new Vector3(311.97f, 103.04f, 311.37f),
                    new Vector3(389.28f, 67.85f, 324.58f),
                    new Vector3(512.60f, 66.30f, 363.10f),
                    new Vector3(524.28f, 65.66f, 371.10f),
                    new Vector3(542.48f, 68.89f, 384.32f),
                    new Vector3(669.50f, 70.00f, 425.84f),
                    new Vector3(675.68f, 70.30f, 435.76f),
                    new Vector3(685.62f, 74.00f, 506.82f),
                ]
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
                happy = true
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
                happy = true,
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
                happy = true,
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
                happy = false,
                customPath = [
                    new Vector3(821.48f, 73.20f, -693.95f),
                    new Vector3(799.20f, 69.64f, -685.00f),
                    new Vector3(803.91f, 73.62f, -607.67f),
                    new Vector3(825.54f, 81.34f, -536.72f),
                    new Vector3(840.46f, 87.94f, -463.74f),
                    new Vector3(808.57f, 96.10f, -311.21f),
                    new Vector3(723.05f, 96.00f, -289.83f),
                ]
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
                happy = true,
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
                happy = true,
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
                happy = true,
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
                happy = true,
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
                happy = true,
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
                happy = true,
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
                happy = true,
            }
        },
    };
}
