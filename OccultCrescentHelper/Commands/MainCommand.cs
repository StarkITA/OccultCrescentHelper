using System.Collections.Generic;
using Ocelot.Modules;
using Ocelot.Commands;

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

    public override IReadOnlyList<string> validArguments => ["config", "cfg"];


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

        plugin.windows?.ToggleMainUI();
    }
}
