
using System.Collections.Generic;
using ECommons.DalamudServices;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.WindowManager;

[OcelotModule]
public class WindowManagerModule : Module<Plugin, Config>
{
    public override WindowManagerConfig config {
        get => _config.WindowManagerConfig;
    }

    public WindowManagerModule(Plugin plugin, Config config)
        : base(plugin, config) { }


    private List<uint> occultCrescentTerritoryIds = [1252];


    public override void PostInitialize()
    {
        if (config.OpenMainOnStartUp)
        {
            plugin.windows.OpenMainUI();
        }


        if (config.OpenConfigOnStartUp)
        {
            plugin.windows.OpenConfigUI();
        }
    }

    public override void OnTerritoryChanged(ushort id)
    {
        if (occultCrescentTerritoryIds.Contains(id))
        {
            if (config.OpenMainOnEnter)
            {
                plugin.windows.OpenMainUI();
            }


            if (config.OpenConfigOnEnter)
            {
                plugin.windows.OpenConfigUI();
            }
        }
        else
        {
            if (config.CloseMainOnExit)
            {
                plugin.windows.CloseMainUI();
            }


            if (config.CloseConfigOnExit)
            {
                plugin.windows.CloseConfigUI();
            }

        }
    }
}
