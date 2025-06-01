using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.Game;
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

        Carrots();
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
        if (ImGui.BeginTable("Treasure", 3, ImGuiTableFlags.SizingFixedFit))
        {
            foreach (var item in TreasureManager.treasure)
            {
                var pos = item.Position;

                var data = Svc
                    .Data.GetExcelSheet<Treasure>()
                    .ToList()
                    .FirstOrDefault(t => t.RowId == item.DataId);

                var type = data.SGB.RowId == 1597 ? "Silver" : "Bronze";

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(
                    $"{type} Treasure Coffer ({pos.X.ToString("F2")}, {pos.Y.ToString("F2")}, {pos.Z.ToString("F2")})"
                );
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

    private void Carrots()
    {
        foreach (var item in CarrotManager.carrots)
        {
            var pos = item.Position;

            ImGui.TextUnformatted(
                $"Carrot: ({pos.X.ToString("F2")}, {pos.Y.ToString("F2")}, {pos.Z.ToString("F2")}"
            );
        }

        if (CarrotManager.carrots.Count <= 0)
        {
            ImGui.TextUnformatted("No nearby carrots.");
        }
    }

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
                ImGui.TextUnformatted($"Active Fate: {data.Name} ({item.Value->Progress}%)");

                if (FatesManager.FateProgress.TryGetValue(id, out var progress) && progress != null)
                {
                    var estimate = progress.EstimateTimeToCompletion();
                    if (estimate != null)
                    {
                        ImGui.SameLine();
                        ImGui.TextUnformatted($"(Est. {estimate.Value:mm\\:ss})");
                    }
                }

                var showDemiatma = data.demiatma != null && plugin.config.ShowDemiatmaDrops;
                if (showDemiatma)
                {
                    var itemData = Svc.Data.GetExcelSheet<Item>().GetRow((uint)data.demiatma);

                    var count = InventoryManager.Instance()->GetInventoryItemCount(itemData.RowId);
                    var needed = Math.Max(0, 3 - 6);

                    var demiatma = Svc
                        .Texture.GetFromGameIcon(new GameIconLookup(itemData.Icon))
                        .GetWrapOrEmpty();

                    ImGui.Indent(20);

                    ImGui.PushStyleColor(
                        ImGuiCol.Border,
                        needed > 0
                            ? new Vector4(0.3f, 0.85f, 0.39f, 1f)
                            : new Vector4(0.95f, 0.26f, 0.21f, 1f)
                    );
                    ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

                    ImGui.BeginChild(
                        "ImageBorder",
                        new Vector2(50, 50),
                        true,
                        ImGuiWindowFlags.NoScrollbar
                    );

                    ImGui.Image(demiatma.ImGuiHandle, new Vector2(48, 48));

                    ImGui.EndChild();

                    ImGui.PopStyleVar();
                    ImGui.PopStyleColor();

                    ImGui.Unindent(20);

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();

                        var label = $"Needed ({needed})";
                        ;
                        if (needed <= 0)
                        {
                            label = $"Not Needed ({count})";
                        }

                        ImGui.TextUnformatted($"{itemData.Name}: {label}");
                        ImGui.EndTooltip();
                    }
                }

                var showNotes = data.notes != null && plugin.config.ShowNoteDrops;
                if (showNotes)
                {
                    var itemData = Svc.Data.GetExcelSheet<Item>().GetRow((uint)data.notes);

                    var demiatma = Svc
                        .Texture.GetFromGameIcon(new GameIconLookup(itemData.Icon))
                        .GetWrapOrEmpty();

                    if (!showDemiatma)
                    {
                        ImGui.Indent(20);
                        ImGui.Image(demiatma.ImGuiHandle, new Vector2(48, 48));
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
