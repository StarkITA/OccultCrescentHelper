using ImGuiNET;

namespace OccultCrescentHelper.Carrots;

public class Panel
{
    public void Draw(CarrotsModule module)
    {
        if (module.tracker.carrots.Count <= 0)
        {
            ImGui.TextUnformatted("No nearby carrots.");
            return;
        }

        foreach (var carrot in module.tracker.carrots)
        {
            if (!carrot.IsValid())
            {
                continue;
            }

            var pos = carrot.GetPosition();
            ImGui.TextUnformatted($"Carrot: ({pos.X:F2}, {pos.Y:F2}, {pos.Z:F2})");
        }
    }
}
