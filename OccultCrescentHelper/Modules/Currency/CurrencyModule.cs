using System;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ImGuiNET;
using OccultCrescentHelper.Modules;

namespace OccultCrescentHelper.Currency;

public class CurrencyModule : Module, IDisposable
{
    public readonly CurrencyTracker tracker = new CurrencyTracker();

    private Panel panel = new Panel();

    public CurrencyConfig config
    {
        get => _config.CurrencyConfig;
    }

    public override bool enabled
    {
        get => config.Enabled;
    }

    public CurrencyModule(Plugin plugin)
        : base(plugin)
    {
        plugin.OnUpdate += Tick;
        Svc.ClientState.TerritoryChanged += TerritoryChanged;
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

    public void TerritoryChanged(ushort territory)
    {
        if (!enabled)
        {
            return;
        }

        tracker.TerritoryChanged(territory);
    }

    public void Dispose()
    {
        plugin.OnUpdate -= Tick;
        Svc.ClientState.TerritoryChanged -= TerritoryChanged;
    }
}
