using System;
using System.Numerics;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using Lumina.Excel.Sheets;

namespace OccultCrescentHelper.Fates;

public class Panel
{
    public void Draw(FatesModule module)
    {
        if (module.tracker.fates.Count <= 0)
        {
            ImGui.TextUnformatted("No active fates.");
            Helpers.Separator();
            return;
        }

        foreach (var fate in module.tracker.fates.Values)
        {
            if (fate == null)
            {
                continue;
            }

            if (!FateTracker.data.TryGetValue(fate.FateId, out var data))
            {
                continue;
            }

            ImGui.TextUnformatted($"Active Fate: {data.Name} ({fate.Progress}%)");

            if (module.tracker.tracker.TryGetValue(fate.FateId, out var progress))
            {
                var estimate = progress.EstimateTimeToCompletion();
                if (estimate != null)
                {
                    ImGui.SameLine();
                    ImGui.TextUnformatted($"(Est. {estimate.Value:mm\\:ss})");
                }
            }

            ImGui.Indent(20);
            var showDemiatma = data.demiatma != null && module.config.ShowDemiatmaDrops;
            if (showDemiatma)
            {
                Demiatma(data);
            }

            var showNotes = data.notes != null && module.config.ShowNoteDrops;
            if (showNotes)
            {
                if (showDemiatma)
                {
                    ImGui.SameLine();
                }

                Notes(data);
            }
            ImGui.Unindent(20);
        }

        Helpers.Separator();
    }

    private unsafe void Demiatma(FateData data)
    {
        var itemData = Svc.Data.GetExcelSheet<Item>().GetRow((uint)data.demiatma);

        var count = InventoryManager.Instance()->GetInventoryItemCount(itemData.RowId);
        var needed = Math.Max(0, 3 - count);

        var border = needed > 0 ? new Vector4(0.3f, 0.85f, 0.39f, 1f) : new Vector4(0.95f, 0.26f, 0.21f, 1f);

        var demiatma = Svc.Texture.GetFromGameIcon(new GameIconLookup(itemData.Icon)).GetWrapOrEmpty();

        DrawIcon(demiatma, border, $"Demiatma_{itemData.RowId}");

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

    private unsafe void Notes(FateData data)
    {
        var itemData = Svc.Data.GetExcelSheet<Item>().GetRow((uint)data.notes);

        var notes = Svc.Texture.GetFromGameIcon(new GameIconLookup(itemData.Icon)).GetWrapOrEmpty();

        DrawIcon(notes, new Vector4(1f, 1f, 1f, 1f), $"Note_{itemData.RowId}");

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted(itemData.Name.ToString());
            ImGui.EndTooltip();
        }
    }

    private void DrawIcon(IDalamudTextureWrap icon, Vector4 border, string id)
    {
        ImGui.PushStyleColor(ImGuiCol.Border, border);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

        ImGui.BeginChild($"ImageBorder##{id}", new Vector2(50, 50), true, ImGuiWindowFlags.NoScrollbar);

        ImGui.Image(icon.ImGuiHandle, new System.Numerics.Vector2(48, 48));

        ImGui.EndChild();

        ImGui.PopStyleVar();
        ImGui.PopStyleColor();
    }
}
