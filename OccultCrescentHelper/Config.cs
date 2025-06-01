using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Configuration;
using ECommons.DalamudServices;

namespace OccultCrescentHelper;

[Serializable]
public class Config : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool SwitchJobOnCombatEnd { get; set; } = false;

    public uint CombatJob { get; set; } = 1;

    public uint ExpJob { get; set; } = 1;

    public bool SwitchToExpJobOnCE { get; set; } = true;

    public bool ShowDemiatmaDrops { get; set; } = true;

    public bool ShowNoteDrops { get; set; } = true;

    public bool DrawLineToBronzeChests { get; set; } = true;

    public bool DrawLineToSilverChests { get; set; } = true;

    public bool DrawLineToCarrots { get; set; } = true;

    public bool ShareObjectPositionData { get; set; } = true;

    public List<Vector3> BronzeTreasureLocations { get; set; } = [];

    public List<Vector3> SilverTreasureLocations { get; set; } = [];

    public List<Vector3> CarrotLocations { get; set; } = [];

    public void Save()
    {
        Svc.PluginInterface.SavePluginConfig(this);
    }
}
