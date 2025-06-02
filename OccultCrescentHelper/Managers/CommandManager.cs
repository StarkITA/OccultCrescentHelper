using System;
using ECommons;

namespace OccultCrescentHelper.Managers;

public class CommandManager : IDisposable
{
    private Plugin plugin;

    public CommandManager(Plugin plugin)
    {
        this.plugin = plugin;

        EzCmd.Add(
            "/och",
            MainCommand,
            @"
Opens Occult Crescent Helper main ui
 - /och : Opens the main ui
 - /och config : opens the config ui
 - /och cfg : opens the config ui
".Trim()
        );

        EzCmd.Add(
            "/ochc",
            ConfigCommand,
            @"
Opens Occult Crescent Helper config ui
 - /ochc : Opens the config ui
".Trim()
        );
    }

    private void MainCommand(string command, string arguments)
    {
        if (arguments == "config" || arguments == "cfg")
        {
            ConfigCommand(command, arguments);
            return;
        }

        plugin.windows.ToggleMainUI();
    }

    private void ConfigCommand(string command, string arguments) => plugin.windows.ToggleConfigUI();

    public void Dispose() { }
}
