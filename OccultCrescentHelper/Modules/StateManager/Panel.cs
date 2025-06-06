using ImGuiNET;
using Ocelot;

namespace OccultCrescentHelper.Modules.StateManager;

public class Panel
{
    public bool Draw(StateManagerModule module)
    {
        if (!module.config.ShowDebug)
        {
            return false;
        }

        OcelotUI.Title("State Manager:");
        OcelotUI.Indent(() => ImGui.TextUnformatted($"State: {module.GetStateText()}"));

        return true;
    }
}
