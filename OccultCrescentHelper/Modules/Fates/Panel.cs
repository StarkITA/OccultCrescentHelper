using System;
using System.Numerics;
using ImGuiNET;
using OccultCrescentHelper.Data;
using OccultCrescentHelper.Enums;
using OccultCrescentHelper.Modules.Teleporter;
using Ocelot;

namespace OccultCrescentHelper.Modules.Fates;

public class Panel
{
    public void Draw(FatesModule module)
    {
        OcelotUI.Title("Fates:");
        OcelotUI.Indent(16, () => {
            if (module.tracker.fates.Count <= 0)
            {
                ImGui.TextUnformatted("No active fates.");
                return;
            }

            foreach (var fate in module.fates.Values)
            {
                if (fate == null)
                {
                    continue;
                }

                if (!EventData.Fates.TryGetValue(fate.FateId, out var data))
                {
                    continue;
                }

                ImGui.TextUnformatted($"{data.Name} ({fate.Progress}%)");


                if (module.progress.TryGetValue(fate.FateId, out var progress))
                {
                    var estimate = progress.EstimateTimeToCompletion();
                    if (estimate != null)
                    {
                        ImGui.SameLine();
                        ImGui.TextUnformatted($"(Est. {estimate.Value:mm\\:ss})");
                    }
                }

                OcelotUI.Indent(16, () => EventIconRenderer.Drops(data, module.plugin.config.EventDropConfig));

                if (module.TryGetModule<TeleporterModule>(out var teleporter) && teleporter!.IsReady())
                {
                    var aethernet = data.aethernet ?? teleporter.GetClosestAethernet(fate.Position);
                    if (ImGui.Button($"Teleport to {aethernet.ToFriendlyString()}##fate_{fate.FateId}"))
                    {
                        Vector3 point = fate.Position;
                        var random = new Random();
                        double angle = random.NextDouble() * Math.PI * 2;
                        double radius = random.NextDouble() * fate.Radius;
                        float offsetX = (float)(Math.Cos(angle) * radius);
                        float offsetZ = (float)(Math.Sin(angle) * radius);
                        point = new Vector3(fate.Position.X + offsetX, fate.Position.Y, fate.Position.Z + offsetZ);

                        teleporter.Teleport(aethernet, point);
                    }
                }
            }
        });
    }
}
