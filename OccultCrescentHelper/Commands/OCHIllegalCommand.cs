using System.Collections.Generic;
using Ocelot.Modules;
using Ocelot.Commands;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using OccultCrescentHelper.Modules.CriticalEncounters;
using OccultCrescentHelper.Modules.Fates;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using OccultCrescentHelper.Data;
using OccultCrescentHelper.Modules.Automator;

namespace OccultCrescentHelper.Commands;

[OcelotCommand]
public class OCHIllegalCommand : OcelotCommand
{
    public override string command => "/ochillegal";

    public override string description => @"
Manage och automator/illegal mode.
 - /ochillegal on (Enables illegal mode (Automation))
 - /ochillegal off (Disables illegal mode (Automation))
 - /ochillegal toggle (Toggles illegal mode (Automation))
--------------------------------
".Trim();

    public override IReadOnlyList<string> validArguments => ["on", "off", "toggle"];

    private readonly Plugin plugin;

    public OCHIllegalCommand(Plugin plugin)
    {
        this.plugin = plugin;
    }

    public override void Command(string command, string arguments)
    {
        if (arguments.Trim() == "")
        {
            arguments = "toggle";
        }

        Svc.Framework.RunOnTick(() => {
            if (!plugin.modules.TryGetModule<AutomatorModule>(out var automator) || automator == null)
            {
                return;
            }

            switch (arguments)
            {
                case "on":
                    automator.config.Enabled = true;
                    break;
                case "off":
                    automator.config.Enabled = false;
                    break;
                case "toggle":
                    automator.config.Enabled = !automator.config.Enabled;
                    break;
            }
        });
    }

    private unsafe void FlagActiveCe(AgentMap* map)
    {
        if (!plugin.modules.TryGetModule<CriticalEncountersModule>(out var source) || source == null)
        {
            return;
        }

        uint index = 0;
        foreach (var encounter in source.criticalEncounters)
        {
            // Skip tower and non register encounters
            if (index == 0 || encounter.State != DynamicEventState.Register)
            {
                index++;
                continue;
            }

            map->SetFlagMapMarker(Svc.ClientState.TerritoryType, Svc.ClientState.MapId, encounter.MapMarker.Position);
            return;
        }
    }

    private unsafe void FlagActiveFate(AgentMap* map, bool ignorePots)
    {
        if (!plugin.modules.TryGetModule<FatesModule>(out var source) || source == null)
        {
            return;
        }

        foreach (var fate in source.fates.Values)
        {
            if (fate == null)
            {
                continue;
            }

            if (!EventData.Fates.TryGetValue(fate.FateId, out var data))
            {
                continue;
            }

            if (ignorePots && data.notes == Enums.MonsterNote.PersistentPots)
            {
                continue;
            }

            map->SetFlagMapMarker(Svc.ClientState.TerritoryType, Svc.ClientState.MapId, data.start ?? fate.Position);
            return;
        }

    }
}
