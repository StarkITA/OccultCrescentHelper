using ECommons.ImGuiMethods;
using ImGuiNET;
using Ocelot;

namespace OccultCrescentHelper.Modules.Exp;

public class Panel
{
    public void Draw(ExpModule module)
    {
        OcelotUI.Title("Exp:");
        OcelotUI.Indent(16, () => {
            // Content here

            if (ImGuiEx.IconButton(Dalamud.Interface.FontAwesomeIcon.Redo, $"Reset##Exp"))
            {
                module.tracker.Reset();
            }

            ImGui.SameLine();
            ImGui.TextUnformatted("Exp per hour");

            ImGui.SameLine();
            ImGui.TextUnformatted(module.tracker.GetExpPerHour().ToString("F2"));
        });
    }
}
