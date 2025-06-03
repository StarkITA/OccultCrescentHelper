using System;
using Dalamud.Interface.Windowing;

namespace OccultCrescentHelper.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Plugin plugin;

    public ConfigWindow(Plugin plugin)
        : base(plugin.Name + " Config")
    {
        this.plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        if (plugin.treasures.config.Draw())
        {
            plugin.config.Save();
        }

        if (plugin.carrots.config.Draw())
        {
            plugin.config.Save();
        }

        if (plugin.currency.config.Draw())
        {
            plugin.config.Save();
        }

        if (plugin.config.EventDropConfig.Draw())
        {
            plugin.config.Save();
        }

        if (plugin.fates.config.Draw())
        {
            plugin.config.Save();
        }

        if (plugin.criticalEncounters.config.Draw())
        {
            plugin.config.Save();
        }

        if (plugin.jobSwitcher.config.Draw())
        {
            plugin.config.Save();
        }

        if (plugin.config.TeleporterConfig.Draw())
        {
            plugin.config.Save();
        }

        if (plugin.api.config.Draw())
        {
            plugin.config.Save();
        }
    }
}
