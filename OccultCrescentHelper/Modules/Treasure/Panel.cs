using ImGuiNET;

namespace OccultCrescentHelper.Treasure;

public class Panel
{
    public void Draw(TreasureModule module)
    {
        if (module.tracker.treasures.Count <= 0)
        {
            ImGui.TextUnformatted("No nearby Treasure.");
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
    }
}
