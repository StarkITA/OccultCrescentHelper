using System;
using System.Linq;
using System.Numerics;
using ECommons.MathHelpers;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using ImGuiNET;
using OccultCrescentHelper.Data;
using OccultCrescentHelper.Enums;
using OccultCrescentHelper.Modules.Teleporter;
using Ocelot;

namespace OccultCrescentHelper.Modules.CriticalEncounters;

public class Panel
{
    public void Draw(CriticalEncountersModule module)
    {
        OcelotUI.Title("Critical Encounters:");
        OcelotUI.Indent(() => {
            var active = module.criticalEncounters.Where(ev => ev.State != DynamicEventState.Inactive).Count();
            if (active <= 0)
            {

                ImGui.TextUnformatted("No active critical engagements.");
                return;
            }


            uint index = 0;
            foreach (var ev in module.criticalEncounters)
            {
                if (ev.State == DynamicEventState.Inactive)
                {
                    index++;
                    continue;
                }

                if (!EventData.CriticalEncounters.TryGetValue(index, out var data))
                {
                    index++;
                    continue;
                }

                ImGui.TextUnformatted(ev.Name.ToString());
                if (index == 0)
                {
                    HandlerTower(ev);
                    index++;
                    continue;
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

                    if (module.progress.TryGetValue(index, out var progress))
                    {
                        var estimate = progress.EstimateTimeToCompletion();
                        if (estimate != null)
                        {
                            ImGui.SameLine();
                            ImGui.TextUnformatted($"(Est. {estimate.Value:mm\\:ss})");
                        }
                    }
                }

                if (ev.State != DynamicEventState.Register)
                {
                    OcelotUI.Indent(() => EventIconRenderer.Drops(data, module.plugin.config.EventDropConfig));
                    index++;
                    continue;
                }

                if (module.TryGetModule<TeleporterModule>(out var teleporter) && teleporter!.IsReady())
                {
                    Vector3 start = ev.MapMarker.Position;

                    teleporter.teleporter.Button(data.aethernet, start, data.Name, $"ce_{index}", data);
                }

                OcelotUI.Indent(() => EventIconRenderer.Drops(data, module.plugin.config.EventDropConfig));

                index++;
            }
        });
    }


    private void HandlerTower(DynamicEvent ev)

    {

    }
}
