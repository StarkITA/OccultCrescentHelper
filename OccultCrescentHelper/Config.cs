using System;
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

    public void Save()
    {
        Svc.PluginInterface.SavePluginConfig(this);
    }
}
