using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using ImGuiNET;
using Lumina.Data.Files;
using Lumina.Excel.Sheets;
using OccultCrescentHelper.Data;
using OccultCrescentHelper.Enums;
using OccultCrescentHelper.Modules.Teleporter;
using Ocelot;
using Ocelot.IPC;

namespace OccultCrescentHelper.Modules.Debug;

public class Panel
{

    public Dictionary<uint, Vector3> FateLocations = [];

    public Panel()
    {
        ProcessLgbData(Svc.ClientState.TerritoryType);
    }

    public void OnTerritoryChanged(ushort id) => ProcessLgbData(id);

    public void ProcessLgbData(ushort id)
    {
        FateLocations.Clear();

        var territorySheet = Svc.Data.GetExcelSheet<TerritoryType>();
        var territoryRow = territorySheet?.GetRow(id);
        if (territoryRow == null)
        {
            Svc.Log.Error($"Could not load TerritoryType for ID {id}");
            return;
        }

        Dictionary<uint, uint> locations = [];
        foreach (var fate in EventData.Fates.Values)
        {
            var fateRow = Svc.Data.GetExcelSheet<Fate>().FirstOrDefault((f) => f.RowId == fate.id);
            locations[fate.id] = fateRow.Location;
        }


        var bg = territoryRow?.Bg.ExtractText();
        var lgbFileName = "bg/" + bg![..(bg!.IndexOf("/level/", StringComparison.Ordinal) + 1)] + "level/planevent.lgb";
        var lgb = Svc.Data.GetFile<LgbFile>(lgbFileName);
        foreach (var layer in lgb?.Layers ?? [])
        {
            foreach (var instanceObject in layer.InstanceObjects)
            {
                if (locations.ContainsValue(instanceObject.InstanceId))
                {
                    var fateId = locations.First(kv => kv.Value == instanceObject.InstanceId).Key;
                    var transform = instanceObject.Transform;
                    var pos = transform.Translation;
                    FateLocations[fateId] = new Vector3(pos.X, pos.Y, pos.Z);
                }
            }
        }
    }

    public void Draw(DebugModule module)
    {
        OcelotUI.Title("Debug:");
        OcelotUI.Indent(() => {
            if (module.TryGetModule<TeleporterModule>(out var teleporter) && teleporter!.IsReady())
            {
                var shards = teleporter.teleporter.GetNearbyAethernetShards();
                if (shards.Count() > 0)
                {
                    OcelotUI.Title("Nearby Aethernet Shards:");
                    OcelotUI.Indent(() => {
                        foreach (var shard in teleporter.teleporter.GetNearbyAethernetShards())
                        {
                            var data = AethernetData.All().FirstOrDefault(o => o.dataId == shard.DataId);
                            ImGui.TextUnformatted(data.aethernet.ToFriendlyString());
                        }
                    });
                }
            }

            if (module.TryGetIPCProvider<VNavmesh>(out var vnav) && vnav!.IsReady())
            {
                OcelotUI.Title("Vnav state:");
                ImGui.SameLine();
                ImGui.TextUnformatted(vnav.IsRunning() ? "Running" : "Pending");
            }

            OcelotUI.Title("Fates:");
            OcelotUI.Indent(() => Fates(module));
            OcelotUI.Title("Critical Encounters:");
            OcelotUI.Indent(() => CriticalEncounters(module));
        });
    }

    private unsafe void Fates(DebugModule module)
    {

        foreach (var data in EventData.Fates.Values)
        {
            if (data.happy)
            {
                continue;
            }

            ImGui.TextUnformatted(data.Name);

            if (module.TryGetModule<TeleporterModule>(out var teleporter) && teleporter!.IsReady())
            {
                var start = FateLocations[data.id];

                teleporter.teleporter.Button(data.aethernet, start, data.Name, $"fate_{data.id}", data);
            }

            OcelotUI.Indent(() => EventIconRenderer.Drops(data, module.plugin.config.EventDropConfig));

            if (data.id != EventData.Fates.Keys.Max())
            {
                OcelotUI.VSpace();
            }
        }
    }

    private unsafe void CriticalEncounters(DebugModule module)
    {
        foreach (var data in EventData.CriticalEncounters.Values)
        {
            if (data.happy)
            {
                continue;
            }

            if (data.customPath == null || data.customPath.Count() > 0)
            {
                continue;
            }

            var ev = PublicContentOccultCrescent.GetInstance()->DynamicEventContainer.Events.ToArray().ToList()[(int)data.id];

            ImGui.TextUnformatted(ev.Name.ToString());

            if (ev.State == DynamicEventState.Inactive)
            {
                ImGui.SameLine();
                ImGui.TextUnformatted($"(Inactive)");
            }

            if (ev.State == DynamicEventState.Register)
            {
                DateTime start = DateTimeOffset.FromUnixTimeSeconds(ev.StartTimestamp).DateTime;
                TimeSpan timeUntilStart = start - DateTime.UtcNow;
                string formattedTime = $"{timeUntilStart.Minutes:D2}:{timeUntilStart.Seconds:D2}";

                ImGui.SameLine();
                ImGui.TextUnformatted($"(Preparing: {formattedTime})");
            }

            if (ev.State == DynamicEventState.Warmup)
            {
                ImGui.SameLine();
                ImGui.TextUnformatted($"(Starting)");
            }

            if (ev.State == DynamicEventState.Battle)
            {
                ImGui.SameLine();
                ImGui.TextUnformatted($"({ev.Progress}%)");
            }

            if (module.TryGetModule<TeleporterModule>(out var teleporter) && teleporter!.IsReady())
            {
                Vector3 start = ev.MapMarker.Position;

                teleporter.teleporter.Button(data.aethernet, start, data.Name, $"ce_{data.id}", data);
            }

            OcelotUI.Indent(() => EventIconRenderer.Drops(data, module.plugin.config.EventDropConfig));

            if (data.id != EventData.CriticalEncounters.Keys.Max())
            {
                OcelotUI.VSpace();
            }
        }
    }
}
