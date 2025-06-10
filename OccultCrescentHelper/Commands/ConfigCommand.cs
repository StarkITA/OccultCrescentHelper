using System.Collections.Generic;
using Ocelot.Modules;
using Ocelot.Commands;

namespace OccultCrescentHelper.Commands;

[OcelotCommand]
public class ConfigCommand : OcelotCommand
{
    public override string command => "/ochc";

    public override string description => @"
Opens Occult Crescent Helper config ui
 - /ochc : Opens the config ui
--------------------------------
".Trim();

    public override IReadOnlyList<string> aliases => ["/occultcrescenthelperconfig"];

    private readonly Plugin plugin;

    public ConfigCommand(Plugin plugin)
    {
        this.plugin = plugin;
    }


    public override void Command(string command, string arguments)
    {
        plugin.windows?.ToggleConfigUI();

    }
}
