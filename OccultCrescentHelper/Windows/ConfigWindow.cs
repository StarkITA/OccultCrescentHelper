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
        var ShowDemiatmaDrops = plugin.config.ShowDemiatmaDrops;
        if (ImGui.Checkbox("Show Demiatma Drops", ref ShowDemiatmaDrops))
        {
            plugin.config.ShowDemiatmaDrops = ShowDemiatmaDrops;
            plugin.config.Save();
        }

        var ShowNoteDrops = plugin.config.ShowNoteDrops;
        if (ImGui.Checkbox("Show Note Drops", ref ShowNoteDrops))
        {
            plugin.config.ShowNoteDrops = ShowNoteDrops;
            plugin.config.Save();
        }

        ImGui.Separator();

        var SwitchJobOnCombatEnd = plugin.config.SwitchJobOnCombatEnd;
        if (ImGui.Checkbox("Switch jobs after combat", ref SwitchJobOnCombatEnd))
        {
            plugin.config.SwitchJobOnCombatEnd = SwitchJobOnCombatEnd;
            plugin.config.Save();
        }

        var Jobs = Svc.Data.GetExcelSheet<MKDSupportJob>().ToList();

        if (plugin.config.SwitchJobOnCombatEnd)
        {
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

                ImGui.EndCombo();
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

                ImGui.EndCombo();
            }

            var SwitchToExpJobOnCE = plugin.config.SwitchToExpJobOnCE;
            if (ImGui.Checkbox("Switch jobs before CE", ref SwitchToExpJobOnCE))
            {
                plugin.config.SwitchToExpJobOnCE = SwitchToExpJobOnCE;
                plugin.config.Save();
            }
        }

        ImGui.Separator();

        var DrawLineToBronzeChests = plugin.config.DrawLineToBronzeChests;
        if (ImGui.Checkbox("Draw line to nearby bronze treasure", ref DrawLineToBronzeChests))
        {
            plugin.config.DrawLineToBronzeChests = DrawLineToBronzeChests;
            plugin.config.Save();
        }

        var DrawLineToSilverChests = plugin.config.DrawLineToSilverChests;
        if (ImGui.Checkbox("Draw line to nearby silver treasure", ref DrawLineToSilverChests))
        {
            plugin.config.DrawLineToSilverChests = DrawLineToSilverChests;
            plugin.config.Save();
        }

        var DrawLineToCarrots = plugin.config.DrawLineToCarrots;
        if (ImGui.Checkbox("Draw line to nearby carrots", ref DrawLineToCarrots))
        {
            plugin.config.DrawLineToCarrots = DrawLineToCarrots;
            plugin.config.Save();
        }

        ImGui.Separator();

        var ShareObjectPositionData = plugin.config.ShareObjectPositionData;
        if (ImGui.Checkbox("Share object position data", ref ShareObjectPositionData))
        {
            plugin.config.ShareObjectPositionData = ShareObjectPositionData;
            plugin.config.Save();
        }
    }
}
