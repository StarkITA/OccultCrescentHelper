using System;
using ECommons.DalamudServices;
using OccultCrescentHelper.Modules.CrowdSourcing;
using OccultCrescentHelper.Modules.EventDrop;
using OccultCrescentHelper.Modules.Teleporter;
using OccultCrescentHelper.Modules.Treasure;
using OccultCrescentHelper.Modules.Carrots;
using OccultCrescentHelper.Modules.Currency;
using OccultCrescentHelper.Modules.CriticalEncounters;
using OccultCrescentHelper.Modules.Fates;

using Ocelot;
using OccultCrescentHelper.Modules.Exp;

namespace OccultCrescentHelper;

[Serializable]
public class Config : IOcelotConfig
{
    public int Version { get; set; } = 1;

    public CrowdSourcingConfig CrowdSourcingConfig { get; set; } = new();

    public TreasureConfig TreasureConfig { get; set; } = new();

    public CarrotsConfig CarrotsConfig { get; set; } = new();

    public CurrencyConfig CurrencyConfig { get; set; } = new();

    public EventDropConfig EventDropConfig { get; set; } = new();

    public FatesConfig FatesConfig { get; set; } = new();

    public CriticalEncountersConfig CriticalEncountersConfig { get; set; } = new();

    public ExpConfig ExpConfig { get; set; } = new();

    public TeleporterConfig TeleporterConfig { get; set; } = new();

    public void Save() => Svc.PluginInterface.SavePluginConfig(this);
}
