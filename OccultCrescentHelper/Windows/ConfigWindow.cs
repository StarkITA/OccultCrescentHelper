using ImGuiNET;
using Ocelot.Modules;
using Ocelot.Windows;

namespace OccultCrescentHelper.Windows;

[OcelotConfigWindow]
public class ConfigWindow : OcelotConfigWindow
{
    public ConfigWindow(Plugin plugin, Config config)
        : base(plugin, config) { }

    // if (plugin.treasures.config.Draw())
    // {
    //     plugin.config.Save();
    // }

    // if (plugin.carrots.config.Draw())
    // {
    //     plugin.config.Save();
    // }

    // if (plugin.currency.config.Draw())
    // {
    //     plugin.config.Save();
    // }

    // if (plugin.config.EventDropConfig.Draw())
    // {
    //     plugin.config.Save();
    // }

    // if (plugin.fates.config.Draw())
    // {
    //     plugin.config.Save();
    // }

    // if (plugin.criticalEncounters.config.Draw())
    // {
    //     plugin.config.Save();
    // }

    // if (plugin.jobSwitcher.config.Draw())
    // {
    //     plugin.config.Save();
    // }

    // if (plugin.config.TeleporterConfig.Draw())
    // {
    //     plugin.config.Save();
    // }

    // if (plugin.api.config.Draw())
    // {
    //     plugin.config.Save();
    // }
    // }
}
