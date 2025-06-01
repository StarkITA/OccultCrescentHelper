using System;
using System.Linq;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ImGuiNET;
using Lumina.Excel.Sheets;

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
        var SwitchJobOnCombatEnd = plugin.config.SwitchJobOnCombatEnd;
        if (ImGui.Checkbox("Switch jobs after combat", ref SwitchJobOnCombatEnd))
        {
            plugin.config.SwitchJobOnCombatEnd = SwitchJobOnCombatEnd;
            plugin.config.Save();
        }

        var Jobs = Svc.Data.GetExcelSheet<MKDSupportJob>().ToList();

        if (plugin.config.SwitchJobOnCombatEnd)
        {
            ImGui.TextUnformatted($"State: {plugin.jobs.GetStateText()}");

            var CombatJob = Jobs.FirstOrDefault(job => job.RowId == plugin.config.CombatJob);
            if (ImGui.BeginCombo("Combat Job", CombatJob.Unknown0.ToString()))
            {
                foreach (var Job in Jobs)
                {
                    if (Job.RowId <= 0)
                    {
                        continue;
                    }

                    var label = Job.Unknown0.ToString();
                    bool isSelected = Job.RowId == plugin.config.CombatJob;

                    if (ImGui.Selectable(label, isSelected))
                    {
                        plugin.config.CombatJob = Job.RowId;
                        plugin.config.Save();
                    }
                }
            }

            var ExpJob = Jobs.FirstOrDefault(job => job.RowId == plugin.config.ExpJob);
            if (ImGui.BeginCombo("Exp Job", ExpJob.Unknown0.ToString()))
            {
                foreach (var Job in Jobs)
                {
                    if (Job.RowId <= 0)
                    {
                        continue;
                    }

                    var label = Job.Unknown0.ToString();
                    bool isSelected = Job.RowId == plugin.config.ExpJob;

                    if (ImGui.Selectable(label, isSelected))
                    {
                        plugin.config.ExpJob = Job.RowId;
                        plugin.config.Save();
                    }
                }
            }
        }
    }
}
