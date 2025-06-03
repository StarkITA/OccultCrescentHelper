using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using ECommons.DalamudServices;
using ECommons.Reflection;
using OccultCrescentHelper.Api;
using OccultCrescentHelper.Carrots;
using OccultCrescentHelper.CriticalEncounters;
using OccultCrescentHelper.Currency;
using OccultCrescentHelper.Fates;
using OccultCrescentHelper.JobSwitcher;
using OccultCrescentHelper.Managers;
using OccultCrescentHelper.MosnterSpawns;
using OccultCrescentHelper.Treasure;

namespace OccultCrescentHelper;

public sealed class Plugin : IDalamudPlugin
{
    internal string Name = "Occult Crescent Helper";

    public Config config { get; init; }

    public readonly CrowdSourcingApi api;

    public readonly Teleporter teleporter;

    // Managers

    public readonly WindowManager windows;

    public readonly CommandManager commands;

    // Tick helper

    public delegate void OnUpdateDelegate(IFramework framework);

    public event OnUpdateDelegate? OnUpdate;

    // Modules
    public readonly TreasureModule treasures;

    public readonly CarrotsModule carrots;

    public readonly FatesModule fates;

    public readonly CriticalEncounterModule criticalEncounters;

    public readonly CurrencyModule currency;

    public readonly JobSwitcherModule jobSwitcher;

    public readonly MosnterSpawnsModule mosnterSpawns;

    public Plugin(IDalamudPluginInterface plugin)
    {
        ECommonsMain.Init(plugin, this, Module.DalamudReflector);
        config = plugin.GetPluginConfig() as Config ?? new Config();

        var gameVersion = DalamudReflector.TryGetDalamudStartInfo(out var ver) ? ver.GameVersion.ToString() : "unknown";
        InteropGenerator.Runtime.Resolver.GetInstance.Setup(
            Svc.SigScanner.SearchBase,
            gameVersion,
            new(Svc.PluginInterface.ConfigDirectory.FullName + "/cs.json")
        );
        FFXIVClientStructs.Interop.Generated.Addresses.Register();
        InteropGenerator.Runtime.Resolver.GetInstance.Resolve();

        DotNetEnv.Env.Load(Svc.PluginInterface.AssemblyLocation.Directory + "/.env");

        api = new CrowdSourcingApi(this);
        teleporter = new Teleporter(this);

        windows = new WindowManager(this);
        commands = new CommandManager(this);

        treasures = new TreasureModule(this);
        carrots = new CarrotsModule(this);
        fates = new FatesModule(this);
        criticalEncounters = new CriticalEncounterModule(this);
        currency = new CurrencyModule(this);
        jobSwitcher = new JobSwitcherModule(this);
        mosnterSpawns = new MosnterSpawnsModule(this);

        Svc.Framework.Update += Tick;

        windows.ToggleConfigUI();
        windows.ToggleMainUI();
    }

    public void Tick(IFramework framework)
    {
        if (!Helpers.IsInOccultCrescent())
        {
            return;
        }

        if (!teleporter.IsReady())
        {
            teleporter.ReadyCheck();
        }

        OnUpdate?.Invoke(framework);
    }

    public void Dispose()
    {
        Svc.Framework.Update -= Tick;

        windows.Dispose();
        commands.Dispose();

        treasures.Dispose();
        carrots.Dispose();
        fates.Dispose();
        criticalEncounters.Dispose();
        currency.Dispose();
        jobSwitcher.Dispose();
        mosnterSpawns.Dispose();

        ECommonsMain.Dispose();
    }
}
