using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using ECommons.DalamudServices;
using OccultCrescentHelper.Api;
using OccultCrescentHelper.Carrots;
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

    public readonly CurrencyModule currency;

    public readonly JobSwitcherModule jobSwitcher;

    public readonly MosnterSpawnsModule mosnterSpawns;

    public Plugin(IDalamudPluginInterface plugin)
    {
        ECommonsMain.Init(plugin, this);
        config = plugin.GetPluginConfig() as Config ?? new Config();

        DotNetEnv.Env.Load(Svc.PluginInterface.AssemblyLocation.Directory + "/.env");

        api = new CrowdSourcingApi(this);

        windows = new WindowManager(this);
        commands = new CommandManager(this);

        treasures = new TreasureModule(this);
        carrots = new CarrotsModule(this);
        fates = new FatesModule(this);
        currency = new CurrencyModule(this);
        jobSwitcher = new JobSwitcherModule(this);
        mosnterSpawns = new MosnterSpawnsModule(this);

        Svc.Framework.Update += Tick;
    }

    public void Tick(IFramework framework)
    {
        if (!Helpers.IsInOccultCrescent())
        {
            return;
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
        currency.Dispose();
        jobSwitcher.Dispose();
        mosnterSpawns.Dispose();

        ECommonsMain.Dispose();
    }
}
