using System;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ImGuiNET;
using OccultCrescentHelper.Modules;

namespace OccultCrescentHelper.Carrots;

public class CarrotsModule : Module, IDisposable
{
    public readonly CarrotsTracker tracker;

    private Radar radar;

    private Panel panel = new Panel();

    public CarrotsConfig config
    {
        get => _config.CarrotsConfig;
    }

    public override bool enabled
    {
        get => config.Enabled;
    }

    public CarrotsModule(Plugin plugin)
        : base(plugin)
    {
        tracker = new CarrotsTracker(plugin.api);
        radar = new Radar(this);

        plugin.OnUpdate += Tick;
        Svc.PluginInterface.UiBuilder.Draw += Radar;
    }

    public void Draw()
    {
        if (!enabled || !Helpers.IsInOccultCrescent())
        {
            return;
        }

        panel.Draw(this);
        ImGui.Separator();
    }

    public void Radar()
    {
        if (!enabled || !Helpers.IsInOccultCrescent())
        {
            return;
        }

        radar.Draw();
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
        Svc.PluginInterface.UiBuilder.Draw -= Radar;
        plugin.OnUpdate -= Tick;
    }
}
