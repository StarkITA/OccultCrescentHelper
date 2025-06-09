using System.Collections.Generic;
using Ocelot.Modules;
using Ocelot.Commands;
using OccultCrescentHelper.Modules.Debug;

namespace OccultCrescentHelper.Commands;

[OcelotCommand]
public class MainCommand : OcelotCommand
{
    public override string command => "/och";

    public override string description => @"
Opens Occult Crescent Helper main ui
    - /och : Opens the main ui
    - /och config : opens the config ui
    - /och cfg : opens the config ui
".Trim();

    public override IReadOnlyList<string> aliases => ["/occultcrescenthelper"];

    public override IReadOnlyList<string> validArguments => ["config", "cfg", "debug"];


    private readonly Plugin plugin;

    public MainCommand(Plugin plugin)
    {
        this.plugin = plugin;
    }


    public override void Command(string command, string arguments)
    {
        if (arguments == "config" || arguments == "cfg")
        {
            plugin.windows?.ToggleConfigUI();
            return;
        }

#if DEBUG_BUILD
        if (arguments == "debug")
        {
            var debug = plugin.modules.GetModule<DebugModule>();
            var window = plugin.windows.GetWindow<DebugWindow>();
            if (debug != null && window != null)
            {
                window.Toggle();
                return;
            }
        }
#endif

        plugin.windows?.ToggleMainUI();
    }
}
