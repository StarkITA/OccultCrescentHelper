using ImGuiNET;
using Ocelot;

namespace OccultCrescentHelper.Modules.InstanceIdentifier;

public class Panel
{
    public void Draw(InstanceIdentifierModule module)
    {
        OcelotUI.Title("Instance Id:");
        ImGui.SameLine();
        ImGui.TextUnformatted(module.instance);
    }
}
