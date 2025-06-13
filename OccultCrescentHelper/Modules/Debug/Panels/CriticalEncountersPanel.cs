using System;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using ImGuiNET;
using OccultCrescentHelper.Data;
using OccultCrescentHelper.Modules.Teleporter;
using Ocelot;

namespace OccultCrescentHelper.Modules.Debug.Panels;

public class CriticalEncountersPanel : Panel
{
    public override string GetName() => "Critical Encounters";

    public unsafe override void Draw(DebugModule module)
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

                OcelotUI.Title("Map Marker:");
                OcelotUI.Indent(() => {
                    OcelotUI.Title("Map Marker:");
                    ImGui.SameLine();
                    ImGui.TextUnformatted(ev.MapMarker.Radius.ToString());
                });
            }
        });
    }
}
