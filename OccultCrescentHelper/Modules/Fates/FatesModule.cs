
using System.Collections.Generic;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Plugin.Services;
using OccultCrescentHelper.Data;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.Fates;

[OcelotModule(7, 5)]
public class FatesModule : Module<Plugin, Config>
{
    public override FatesConfig config {
        get => _config.FatesConfig;
    }

    public readonly FateTracker tracker = new();

    public Dictionary<uint, IFate> fates => tracker.fates;

    public Dictionary<uint, EventProgress> progress => tracker.progress;

    private Panel panel = new();

    public FatesModule(Plugin plugin, Config config)
        : base(plugin, config) { }

    public override void Tick(IFramework framework) => tracker.Tick(framework);

    public override bool DrawMainUi()
    {
        panel.Draw(this);
        return true;
    }
}
