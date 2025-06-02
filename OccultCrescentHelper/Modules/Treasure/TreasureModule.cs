using System;
using System.Numerics;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ImGuiNET;
using OccultCrescentHelper.Modules;

namespace OccultCrescentHelper.Treasure;

public class TreasureModule : Module, IDisposable
{
    public readonly TreasureTracker tracker;

    public static Vector4 bronze = new Vector4(0.804f, 0.498f, 0.196f, 1f);

    public static Vector4 silver = new Vector4(0.753f, 0.753f, 0.753f, 1f);

    public static Vector4 unknown = new Vector4(0.6f, 0.2f, 0.8f, 1f);

    private Panel panel = new Panel();

    private Radar radar;

    public TreasureConfig config
    {
        get => _config.TreasureConfig;
    }

    public override bool enabled
    {
        get => config.Enabled;
    }

    public TreasureModule(Plugin plugin)
        : base(plugin)
    {
        tracker = new TreasureTracker(plugin.api);
        radar = new Radar(this);

        plugin.OnUpdate += Tick;
        Svc.PluginInterface.UiBuilder.Draw += Radar;
    }

    public void Tick(IFramework framework)
    {
        if (!enabled)
        {
            return;
        }

        tracker.Tick(framework);
    }

    public void Draw()
    {
        if (!enabled)
        {
            return;
        }

        panel.Draw(this);
        ImGui.Separator();
    }

    public void Radar()
    {
        if (!enabled)
        {
            return;
        }

        radar.Draw();
    }

    public void Dispose()
    {
        Svc.PluginInterface.UiBuilder.Draw -= Draw;
        plugin.OnUpdate -= Tick;
    }
}
