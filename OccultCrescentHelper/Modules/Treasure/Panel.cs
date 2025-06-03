using System.Numerics;
using ImGuiNET;

namespace OccultCrescentHelper.Treasure;

public class Panel
{
    public void Draw(TreasureModule module)
    {
        ImGui.TextColored(new Vector4(1f, 0.75f, 0.25f, 1f), "Treasure:");
        ImGui.Indent(16);

        if (module.tracker.treasures.Count <= 0)
        {
            ImGui.TextUnformatted("No nearby Treasure.");
            ImGui.Unindent(16);
            Helpers.VSpace();
            return;
        }

        if (ImGui.BeginTable("Treasure", 2, ImGuiTableFlags.SizingFixedFit))
        {
            int index = 0;
            foreach (var treasure in module.tracker.treasures)
            {
                if (!treasure.IsValid())
                {
                    continue;
                }

                var pos = treasure.GetPosition();

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.TextUnformatted($"{treasure.GetName()} ({pos.X:F2}, {pos.Y:F2}, {pos.Z:F2})");

                ImGui.TableNextColumn();
                if (ImGui.Button($"Target###{index}"))
                {
                    treasure.Target();
                }

                index++;
            }

            ImGui.EndTable();
        }

        ImGui.Unindent(16);
        Helpers.VSpace();
    }
}
