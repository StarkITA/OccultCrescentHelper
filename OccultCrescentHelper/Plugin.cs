using Dalamud.Plugin;
using ECommons;
using ECommons.DalamudServices;
using ECommons.Reflection;
using Ocelot;

namespace OccultCrescentHelper;

public sealed class Plugin : OcelotPlugin
{
    public override string Name {
        get => "Occult Crescent Helper";
    }

    public Config config { get; init; }

    public override IOcelotConfig _config => config;

    public Plugin(IDalamudPluginInterface plugin)
        : base(plugin, Module.DalamudReflector)
    {
        config = plugin.GetPluginConfig() as Config ?? new Config();
        InitializeClientStructs();
        OcelotInitialize();

        DotNetEnv.Env.Load(Svc.PluginInterface.AssemblyLocation.Directory + "/.env");
    }

    private void InitializeClientStructs()
    {
        var gameVersion = DalamudReflector.TryGetDalamudStartInfo(out var ver) ? ver.GameVersion!.ToString() : "unknown";
        InteropGenerator.Runtime.Resolver.GetInstance.Setup(
            Svc.SigScanner.SearchBase,
            gameVersion,
            new(Svc.PluginInterface.ConfigDirectory.FullName + "/cs.json")
        );
        FFXIVClientStructs.Interop.Generated.Addresses.Register();
        InteropGenerator.Runtime.Resolver.GetInstance.Resolve();
    }


    public override bool ShouldTick() => Helpers.IsInOccultCrescent() && !GenericHelpers.IsOccupied();
}
