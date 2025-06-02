using Dalamud.Game.ClientState.Fates;
using OccultCrescentHelper.Enums;

namespace OccultCrescentHelper.Fates;

public struct FateData
{
    public IFate? fate;

    public uint id;

    public string Name;

    public Demiatma? demiatma;

    public SoulShard? soulshard;

    public MonsterNote? notes;

    public Monster? monster;
}
