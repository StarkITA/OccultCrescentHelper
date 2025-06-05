using OccultCrescentHelper.Modules.CrowdSourcing.Api;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.CrowdSourcing;

[OcelotModule(9)]
public class CrowdSourcingModule : Module<Plugin, Config>
{
    public readonly CrowdSourcingApi api;

    public override CrowdSourcingConfig config {
        get => _config.CrowdSourcingConfig;
    }

    public CrowdSourcingModule(Plugin plugin, Config config)
        : base(plugin, config)
    {
        api = new(plugin);
    }
}
