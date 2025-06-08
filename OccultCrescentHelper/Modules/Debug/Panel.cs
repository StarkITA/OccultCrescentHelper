using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ECommons.Throttlers;
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

    private enum PathType
    {
        Move,
        Jump,
    }

    private class PathEntry
    {
        public Vector3 pos;
        public PathType type;

        public PathEntry(Vector3 pos, PathType type)
        {
            this.pos = pos;
            this.type = type;
        }

        public static PathEntry Move(Vector3 pos) => new(pos, PathType.Move);
        public static PathEntry Jump(Vector3 pos) => new(pos, PathType.Jump);

        public override bool Equals(object? obj)
        {
            return obj is PathEntry other &&
                   pos.Equals(other.pos) &&
                   type == other.type;
        }

        public override int GetHashCode() => HashCode.Combine(pos, type);
    }

    private List<PathEntry> path = [];

    private bool running = false;

    public Panel()
    {
        ProcessLgbData(Svc.ClientState.TerritoryType);
    }

    public void OnTerritoryChanged(ushort id) => ProcessLgbData(id);

    public void ProcessLgbData(ushort id)
    {
        if (id == 0)
        {
            return;
        }

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
        OcelotUI.Region("OCH##DEbug", () => {
            OcelotUI.Title("Debug:");
            OcelotUI.Indent(() => {
                if (ImGui.CollapsingHeader("Teleporter"))
                {
                    Teleporter(module);
                    OcelotUI.VSpace();
                }

                if (ImGui.CollapsingHeader("VNav"))
                {
                    VNav(module);
                    OcelotUI.VSpace();
                }

                if (ImGui.CollapsingHeader("Fates"))
                {
                    Fates(module);
                    OcelotUI.VSpace();
                }

                if (ImGui.CollapsingHeader("CriticalEncounters"))
                {
                    CriticalEncounters(module);
                    OcelotUI.VSpace();
                }

                if (ImGui.CollapsingHeader("PathMaker"))
                {
                    PathMaker(module);
                    OcelotUI.VSpace();
                }
            });
        });
    }

    private bool jumping = false;

    public void Tick(DebugModule module)
    {
        if (!running)
        {
            return;
        }

        Vector3 snap(Vector3 pos) => new(
            MathF.Round(pos.X, 2),
            MathF.Round(pos.Y, 2),
            MathF.Round(pos.Z, 2)
        );

        if (EzThrottler.Throttle("Path Generator", 100))
        {
            var player = Svc.ClientState.LocalPlayer;
            if (player == null)
            {
                return;
            }

            var jumpCondition = Svc.Condition[ConditionFlag.Jumping];
            var position = snap(player.Position);

            if (!jumping && jumpCondition)
            {
                var jump = PathEntry.Jump(position);
                if (!path.Contains(jump))
                {
                    path.Add(jump);
                }
                jumping = true;
            }

            if (jumpCondition)
            {
                return;
            }

            jumping = false;

            var move = PathEntry.Move(position);
            if (!path.Contains(move))
            {
                path.Add(move);
            }
        }
    }

    private unsafe void PathMaker(DebugModule module)
    {
        OcelotUI.Title("Path Maker:");
        OcelotUI.Indent(() => {
            var label = running ? "Stop" : "Start";

            if (ImGui.Button(label))
            {
                running = !running;
                if (running)
                {
                    // Clear on start
                    path.Clear();
                }
            }

            var moveNodes = path.Where(e => e.type == PathType.Move).Select(e => $"[{e.pos.X:f2}f, {e.pos.Y:f2}f, {e.pos.Z:f2}f]");
            var jumpNodes = path.Where(e => e.type == PathType.Jump).Select(e => $"[{e.pos.X:f2}f, {e.pos.Y:f2}f, {e.pos.Z:f2}f]");

            var output = "Prowler.PathWithJumps([\n    " +
                string.Join(",\n    ", moveNodes) +
            "\n], [\n    " +
                string.Join(",\n    ", jumpNodes) +
            "\n]),";

            // var output = string.Join("\n", nodes);
            ImGui.PushItemWidth(-1); // Use all available width
            ImGui.InputTextMultiline("##PathOutput", ref output, 4096, ImGui.GetContentRegionAvail(), ImGuiInputTextFlags.ReadOnly);
            ImGui.PopItemWidth();
        });
    }


    private unsafe void Teleporter(DebugModule module)
    {
        if (module.TryGetModule<TeleporterModule>(out var teleporter) && teleporter!.IsReady())
        {
            OcelotUI.Title("Teleporter:");
            OcelotUI.Indent(() => {
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

                if (ImGui.Button("Test Return"))
                {
                    teleporter.teleporter.Return();
                }
            });
        }
    }

    private unsafe void VNav(DebugModule module)
    {
        if (module.TryGetIPCProvider<VNavmesh>(out var vnav) && vnav!.IsReady())
        {
            OcelotUI.Title("Vnav state:");
            ImGui.SameLine();
            ImGui.TextUnformatted(vnav.IsRunning() ? "Running" : "Pending");
        }
    }

    private unsafe void Fates(DebugModule module)
    {
        OcelotUI.Title("Fates:");
        OcelotUI.Indent(() => {
            foreach (var data in EventData.Fates.Values)
            {
                ImGui.TextUnformatted(data.Name);

                if (module.TryGetModule<TeleporterModule>(out var teleporter) && teleporter!.IsReady())
                {
                    var start = FateLocations[data.id];

                    teleporter.teleporter.Button(data.aethernet, start, data.Name, $"fate_{data.id}", data);
                }

                OcelotUI.Indent(() => EventIconRenderer.Drops(data, module.plugin.config.EventDropConfig));

                if (data.pathFactory != null)
                {
                    ImGui.TextColored(new Vector4(0.5f, 1.0f, 0.5f, 1.0f), "Has custom path");
                }

                if (data.id != EventData.Fates.Keys.Max())
                {
                    OcelotUI.VSpace();
                }
            }
        });
    }

    private unsafe void CriticalEncounters(DebugModule module)
    {
        OcelotUI.Title("Critical Encounters:");
        OcelotUI.Indent(() => {
            foreach (var data in EventData.CriticalEncounters.Values)
            {
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

                if (data.pathFactory != null)
                {
                    ImGui.TextColored(new Vector4(0.5f, 1.0f, 0.5f, 1.0f), "Has custom path");
                }

                if (data.id != EventData.CriticalEncounters.Keys.Max())
                {
                    OcelotUI.VSpace();
                }
            }
        });
    }
}
