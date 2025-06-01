using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using ImGuiNET;
using Lumina.Excel.Sheets;
using OccultCrescentHelper.Managers;
using OccultCrescentHelper.Trackers;

namespace OccultCrescentHelper.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin plugin;

    public MainWindow(Plugin plugin)
        : base(plugin.Name, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        Treasure();
        ImGui.Separator();

        List<Tracked> tracked = plugin.trackers.GetData();
        if (ImGui.BeginTable("TrackedData", 3, ImGuiTableFlags.SizingStretchSame))
        {
            foreach (var item in tracked)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(item.label);
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(item.GetRatePerHour().ToString("F2"));

                ImGui.TableNextColumn();
                if (
                    ImGuiEx.IconButton(
                        Dalamud.Interface.FontAwesomeIcon.Redo,
                        $"Reset##{item.label}"
                    )
                )
                {
                    item.Reset(item.value);
                }
            }

            ImGui.EndTable();
        }

        ImGui.Separator();
        Fates();
    }

    private void Treasure()
    {
        if (ImGui.BeginTable("Treasure", 3, ImGuiTableFlags.SizingStretchSame))
        {
            foreach (var item in TreasureManager.treasure)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(item.Position.ToString());
                ImGui.TableNextColumn();
                if (ImGui.Button($"Target###{item.DataId}"))
                {
                    Svc.Targets.Target = item;
                }
            }

            ImGui.EndTable();
        }

        if (TreasureManager.treasure.Count <= 0)
        {
            ImGui.TextUnformatted("No nearby treasure.");
        }
    }

    // private ITextureProvider

    private unsafe void Fates()
    {
        if (FatesManager.fates.Count <= 0)
        {
            ImGui.TextUnformatted("No active fates.");
            return;
        }

        foreach (var item in FatesManager.fates)
        {
            uint id = item.Value->FateId;

            if (FatesManager.FateData.TryGetValue(id, out var data))
            {
                ImGui.TextUnformatted($"Active Fate: {data.Name}");

                if (data.demiatma != null)
                {
                    var itemData = Svc.Data.GetExcelSheet<Item>().GetRow((uint)data.demiatma);

                    var demiatma = Svc
                        .Texture.GetFromGameIcon(new GameIconLookup(itemData.Icon))
                        .GetWrapOrEmpty();

                    ImGui.Indent(20);
                    ImGui.Image(demiatma.ImGuiHandle, new Vector2(48, 48));
                    ImGui.Unindent(20);

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(itemData.Name.ToString());
                        ImGui.EndTooltip();
                    }
                }

                if (data.notes != null)
                {
                    var itemData = Svc.Data.GetExcelSheet<Item>().GetRow((uint)data.notes);

                    var demiatma = Svc
                        .Texture.GetFromGameIcon(new GameIconLookup(itemData.Icon))
                        .GetWrapOrEmpty();

                    if (data.demiatma == null)
                    {
                        ImGui.Indent(20);
                        ImGui.Unindent(20);
                    }
                    else
                    {
                        ImGui.SameLine();
                        ImGui.Image(demiatma.ImGuiHandle, new Vector2(48, 48));
                    }

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(itemData.Name.ToString());
                        ImGui.EndTooltip();
                    }
                }
            }
            else
            {
                ImGui.TextUnformatted(item.Value->Name.ToString());
            }
        }
    }
}
