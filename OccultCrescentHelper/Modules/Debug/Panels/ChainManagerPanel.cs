using ImGuiNET;
using Ocelot;
using Ocelot.Chain;

namespace OccultCrescentHelper.Modules.Debug.Panels;

public class ChainManagerPanel : Panel
{
    public override string GetName() => "Chain Manager";

    public override void Draw(DebugModule module)
    {
        OcelotUI.Title("Chain Manager:");
        OcelotUI.Indent(() => {
            OcelotUI.Title("Is Running:");
            ImGui.SameLine();
            ImGui.TextUnformatted(ChainManager.IsRunning() ? "Yes" : "No");

            if (!Ocelot.Chain.ChainManager.IsRunning())
            {
                return;
            }

            OcelotUI.Title("Current Chain:");
            ImGui.SameLine();
            ImGui.TextUnformatted(ChainManager.Chain()!.name);

            OcelotUI.Title("Progress:");
            ImGui.SameLine();
            ImGui.TextUnformatted($"{ChainManager.Chain()!.progress * 100}%");

            OcelotUI.Title("Queued Chains:");
            ImGui.SameLine();
            ImGui.TextUnformatted(ChainManager.GetChainQueueCount().ToString());
        });
    }
}
