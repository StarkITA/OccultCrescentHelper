using System.Numerics;
using ECommons.DalamudServices;
using ImGuiNET;

namespace OccultCrescentHelper;

public static class Helpers
{
    public static bool IsInSouthHorn()
    {
        return Svc.ClientState.TerritoryType == 1252;
    }

    public static bool IsInOccultCrescent()
    {
        return Svc.ClientState.LocalPlayer != null && IsInSouthHorn();
    }

    public static void DrawLine(Vector3 start, Vector3 end, float thickness, Vector4 color)
    {
        bool startValid = Svc.GameGui.WorldToScreen(start, out Vector2 startScreen);
        bool endValid = Svc.GameGui.WorldToScreen(end, out Vector2 endScreen);

        if (startValid && endValid)
        {
            var imguiColor = ImGui.ColorConvertFloat4ToU32(color);
            ImGui.GetBackgroundDrawList().AddLine(startScreen, endScreen, imguiColor, thickness);
        }
    }

    public static void Separator()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
        var pos = ImGui.GetCursorScreenPos();
        var width = ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X;
        var drawList = ImGui.GetWindowDrawList();

        drawList.AddLine(new Vector2(pos.X, pos.Y), new Vector2(pos.X + width, pos.Y), ImGui.GetColorU32(ImGuiCol.Separator));

        ImGui.Dummy(new Vector2(width, 1));

        ImGui.PopStyleVar();
    }

    public static void VSpace(int px = 8)
    {
        ImGui.Dummy(new Vector2(0, px));
    }
}
