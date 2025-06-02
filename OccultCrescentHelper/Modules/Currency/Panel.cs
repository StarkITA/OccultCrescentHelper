using ECommons.ImGuiMethods;
using ImGuiNET;

namespace OccultCrescentHelper.Currency;

public class Panel
{
    public void Draw(CurrencyModule module)
    {
        if (ImGui.BeginTable("CurrencyData##OCH", 3, ImGuiTableFlags.SizingFixedFit))
        {
            // Silver
            ImGui.TableNextRow();

            ImGui.TableNextColumn();
            if (ImGuiEx.IconButton(Dalamud.Interface.FontAwesomeIcon.Redo, $"Reset##Silver"))
            {
                module.tracker.ResetSilver();
            }

            ImGui.TableNextColumn();
            ImGui.TextUnformatted("Silver per hour");

            ImGui.TableNextColumn();
            ImGui.TextUnformatted(module.tracker.GetSilverPerHour().ToString("F2"));

            // Gold
            ImGui.TableNextRow();

            ImGui.TableNextColumn();
            if (ImGuiEx.IconButton(Dalamud.Interface.FontAwesomeIcon.Redo, $"Reset##Gold"))
            {
                module.tracker.ResetGold();
            }

            ImGui.TableNextColumn();
            ImGui.TextUnformatted("Gold per hour");

            ImGui.TableNextColumn();
            ImGui.TextUnformatted(module.tracker.GetGoldPerHour().ToString("F2"));

            ImGui.EndTable();
        }

        Helpers.Separator();
    }
}
