using System;
using Dalamud.Configuration;
using ECommons.DalamudServices;
using OccultCrescentHelper.Carrots;
using OccultCrescentHelper.CriticalEncounters;
using OccultCrescentHelper.Currency;
using OccultCrescentHelper.Fates;
using OccultCrescentHelper.JobSwitcher;
using OccultCrescentHelper.Treasure;

namespace OccultCrescentHelper;

[Serializable]
public class Config : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    public CrowdSourcingConfig CrowdSourcingConfig { get; set; } = new();

    public CarrotsConfig CarrotsConfig { get; set; } = new();

    public TreasureConfig TreasureConfig { get; set; } = new();

    public CurrencyConfig CurrencyConfig { get; set; } = new();

    public EventDropConfig EventDropConfig { get; set; } = new();

    public FatesConfig FatesConfig { get; set; } = new();

    public CriticalEncounterConfig CriticalEncounterConfig { get; set; } = new();

    public JobSwitcherConfig JobSwitcherConfig { get; set; } = new();

    public void Save() => Svc.PluginInterface.SavePluginConfig(this);
}
