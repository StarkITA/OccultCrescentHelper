using System;
using System.Numerics;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using ImGuiNET;
using Lumina.Excel.Sheets;
using OccultCrescentHelper.Enums;

namespace OccultCrescentHelper.CriticalEncounters;

public class Panel
{
    public void Draw(CriticalEncounterModule module)
    {
        var active = 0;
        uint id = 0;

        ImGui.TextColored(new Vector4(1f, 0.75f, 0.25f, 1f), "Critical Encounters:");
        ImGui.Indent(16);

        foreach (var ce in module.tracker.criticalEncounters)
        {
            if (ce.State == DynamicEventState.Inactive)
            {
                id++;
                continue;
            }

            if (!CriticalEncounterTracker.data.TryGetValue(id, out var data))
            {
                id++;
                continue;
            }

            active++;

            ImGui.TextUnformatted(ce.Name.ToString());

            if (ce.State == DynamicEventState.Register && id > 0)
            {
                DateTime start = DateTimeOffset.FromUnixTimeSeconds(ce.StartTimestamp).DateTime;
                TimeSpan timeUntilStart = start - DateTime.UtcNow;
                string formattedTime = $"{timeUntilStart.Minutes:D2}:{timeUntilStart.Seconds:D2}";

                ImGui.SameLine();
                ImGui.TextUnformatted($"(Preparing: {formattedTime})");
            }

            if (ce.State == DynamicEventState.Warmup)
            {
                ImGui.SameLine();
                ImGui.TextUnformatted($"(Starting)");
            }

            if (ce.State == DynamicEventState.Battle)
            {
                ImGui.SameLine();
                ImGui.TextUnformatted($"({ce.Progress}%)");

                if (module.tracker.tracker.TryGetValue(id, out var progress))
                {
                    var estimate = progress.EstimateTimeToCompletion();
                    if (estimate != null)
                    {
                        ImGui.SameLine();
                        ImGui.TextUnformatted($"(Est. {estimate.Value:mm\\:ss})");
                    }
                }
            }

            ImGui.Indent(16);
            var showDemiatma = data.demiatma != null && module._config.EventDropConfig.ShowDemiatmaDrops;
            if (showDemiatma)
            {
                Demiatma(data);
            }

            var showNotes = data.notes != null && module._config.EventDropConfig.ShowNoteDrops;
            if (showNotes)
            {
                if (showDemiatma)
                {
                    ImGui.SameLine();
                }

                Notes(data);
            }

            var showSoulShard = data.soulshard != null && module._config.EventDropConfig.ShowSoulShardDrops;
            if (showSoulShard)
            {
                if (showDemiatma || showNotes)
                {
                    ImGui.SameLine();
                }

                SoulShard(data);
            }

            ImGui.Unindent(16);

            if (module.plugin.teleporter.IsReady() && ce.State == DynamicEventState.Register)
            {
                var aethernet = module.plugin.teleporter.GetClosestAethernet(ce.MapMarker.Position);
                if (ImGui.Button($"Tekeport to {aethernet.ToFriendlyString()}##ce_{id}"))
                {
                    module.plugin.teleporter.Teleport(aethernet);
                }
            }

            id++;
        }

        if (active == 0)
        {
            ImGui.TextUnformatted("No active critical engagements.");
        }

        Helpers.VSpace();
        ImGui.Unindent(16);
        Helpers.Separator();
    }

    private unsafe void Demiatma(CriticalEncounterData data)
    {
        var itemData = Svc.Data.GetExcelSheet<Item>().GetRow((uint)data.demiatma);

        var count = InventoryManager.Instance()->GetInventoryItemCount(itemData.RowId);
        var needed = Math.Max(0, 3 - count);

        var border = needed > 0 ? new Vector4(0.3f, 0.85f, 0.39f, 1f) : new Vector4(0.95f, 0.26f, 0.21f, 1f);

        var demiatma = Svc.Texture.GetFromGameIcon(new GameIconLookup(itemData.Icon)).GetWrapOrEmpty();

        DrawIcon(demiatma, border, $"Demiatma_{itemData.RowId}_ce_{data.id}");

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

    private unsafe void Notes(CriticalEncounterData data)
    {
        var itemData = Svc.Data.GetExcelSheet<Item>().GetRow((uint)data.notes);

        var notes = Svc.Texture.GetFromGameIcon(new GameIconLookup(itemData.Icon)).GetWrapOrEmpty();

        DrawIcon(notes, new Vector4(1f, 1f, 1f, 1f), $"Note_{itemData.RowId}_ce_{data.id}");

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted(itemData.Name.ToString());
            ImGui.EndTooltip();
        }
    }

    private unsafe void SoulShard(CriticalEncounterData data)
    {
        var itemData = Svc.Data.GetExcelSheet<Item>().GetRow((uint)data.soulshard);

        var soulshard = Svc.Texture.GetFromGameIcon(new GameIconLookup(itemData.Icon)).GetWrapOrEmpty();

        DrawIcon(soulshard, new Vector4(1f, 1f, 1f, 1f), $"SoulShard_{itemData.RowId}_ce_{data.id}");

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
