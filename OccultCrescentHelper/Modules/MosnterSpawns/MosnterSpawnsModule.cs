using System;
using System.Numerics;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ImGuiNET;
using OccultCrescentHelper.Modules;

namespace OccultCrescentHelper.MosnterSpawns;

public class MosnterSpawnsModule : Module, IDisposable
{
    public readonly MosnterSpawnsTracker tracker;

    public MosnterSpawnsModule(Plugin plugin)
        : base(plugin)
    {
        tracker = new MosnterSpawnsTracker(plugin.api);

        plugin.OnUpdate += Tick;
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
