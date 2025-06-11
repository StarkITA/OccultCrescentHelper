using ImGuiNET;
using Ocelot;

namespace OccultCrescentHelper.Modules.Automator;

public class Panel
{
    public void Draw(AutomatorModule module)
    {
        OcelotUI.Title("Illegal Mode:");
        OcelotUI.Indent(() => {
            OcelotUI.Title("Activity:");
            ImGui.SameLine();
            ImGui.TextUnformatted(module.automator.activity?.data.Name ?? "None");

            OcelotUI.Title("Activity State:");
            ImGui.SameLine();
            ImGui.TextUnformatted(module.automator.activity?.state.ToLabel() ?? "Waiting for Activity");
        });
    }
}
