using OccultCrescentHelper.Enums;

namespace OccultCrescentHelper.CriticalEncounters;

public struct CriticalEncounterData
{
    public uint id;

    public string Name;

    public Demiatma? demiatma;

    public SoulShard? soulshard;

    public MonsterNote? notes;

    public Monster? monster;
}
