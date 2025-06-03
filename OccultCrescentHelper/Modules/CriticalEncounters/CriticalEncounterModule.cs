using System;
using Dalamud.Plugin.Services;
using OccultCrescentHelper.Modules;

namespace OccultCrescentHelper.CriticalEncounters;

public class CriticalEncounterModule : Module, IDisposable
{
    public readonly CriticalEncounterTracker tracker = new CriticalEncounterTracker();

    private Panel panel = new Panel();

    public CriticalEncounterConfig config
    {
        get => _config.CriticalEncounterConfig;
    }

    public override bool enabled
    {
        get => config.Enabled;
    }

    public CriticalEncounterModule(Plugin plugin)
        : base(plugin)
    {
        plugin.OnUpdate += Tick;
    }

    public void Draw()
    {
        if (!enabled || !Helpers.IsInOccultCrescent())
        {
            return;
        }

        panel.Draw(this);
    }

    public void Tick(IFramework framework)
    {
        if (!enabled)
        {
            return;
        }

        tracker.Tick(framework);
    }

    public void Dispose()
    {
        plugin.OnUpdate -= Tick;
    }
}
