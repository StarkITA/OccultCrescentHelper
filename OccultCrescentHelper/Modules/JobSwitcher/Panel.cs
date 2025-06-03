using System.Numerics;
using ImGuiNET;

namespace OccultCrescentHelper.JobSwitcher;

public class Panel
{
    public void Draw(JobSwitcherModule module)
    {
        if (!module.config.ShowStateDebug)
        {
            return;
        }

        ImGui.TextColored(new Vector4(1f, 0.75f, 0.25f, 1f), "Content Handler:");
        ImGui.Indent(16);

        ImGui.TextUnformatted($"State: {module.switcher.GetStateText()}");

        ImGui.Unindent(16);
        Helpers.VSpace();
        Helpers.Separator();
    }
}
