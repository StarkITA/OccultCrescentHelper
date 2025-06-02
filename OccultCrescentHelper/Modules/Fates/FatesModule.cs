using System;
using Dalamud.Plugin.Services;
using ImGuiNET;
using OccultCrescentHelper.Modules;

namespace OccultCrescentHelper.Fates;

public class FatesModule : Module, IDisposable
{
    public readonly FateTracker tracker = new FateTracker();

    private Panel panel = new Panel();

    public FatesConfig config
    {
        get => _config.FatesConfig;
    }

    public override bool enabled
    {
        get => config.Enabled;
    }

    public FatesModule(Plugin plugin)
        : base(plugin)
    {
        plugin.OnUpdate += Tick;
    }

    public void Draw()
    {
        if (!enabled)
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
