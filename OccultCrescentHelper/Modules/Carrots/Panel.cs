using ImGuiNET;
using Ocelot;

namespace OccultCrescentHelper.Modules.Carrots;


public class Panel
{
    public void Draw(CarrotsModule module)
    {
        OcelotUI.Title("Carrots:");
        OcelotUI.Indent(16, () => {
            if (module.carrots.Count <= 0)
            {
                ImGui.TextUnformatted("No nearby carrots.");
                return;
            }

            foreach (var carrot in module.carrots)
            {
                if (!carrot.IsValid())
                {
                    continue;
                }

                var pos = carrot.GetPosition();
                ImGui.TextUnformatted($"Carrot: ({pos.X:F2}, {pos.Y:F2}, {pos.Z:F2})");
            }
        });
    }
}
