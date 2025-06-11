
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.Automator;

[OcelotModule]
public class AutomatorModule : Module<Plugin, Config>
{
    public override AutomatorConfig config {
        get => _config.AutomatorConfig;
    }

    public override bool enabled => config.IsPropertyEnabled(nameof(config.Enabled));

    public readonly Automator automator = new();

    private Panel panel = new();

    private List<uint> occultCrescentTerritoryIds = [1252];

    public AutomatorModule(Plugin plugin, Config config)
        : base(plugin, config) { }


    public override void Tick(IFramework _) => automator.Tick(this);


    public override bool DrawMainUi()
    {
        panel.Draw(this);
        return true;
    }

    public override void OnTerritoryChanged(ushort id)
    {
        if (occultCrescentTerritoryIds.Contains(id))
        {
            return;
        }

        config.Enabled = false;
        plugin.config.Save();
    }
}
