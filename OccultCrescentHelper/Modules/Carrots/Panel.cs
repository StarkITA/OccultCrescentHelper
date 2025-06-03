using System.Numerics;
using ImGuiNET;

namespace OccultCrescentHelper.Carrots;

public class Panel
{
    public void Draw(CarrotsModule module)
    {
        ImGui.TextColored(new Vector4(1f, 0.75f, 0.25f, 1f), "Carrots:");
        ImGui.Indent(16);
        if (module.tracker.carrots.Count <= 0)
        {
            ImGui.TextUnformatted("No nearby carrots.");
            ImGui.Unindent(16);
            Helpers.VSpace();
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

        ImGui.Unindent(16);
        Helpers.VSpace();
    }
}
