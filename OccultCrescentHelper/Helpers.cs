using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using ImGuiNET;

namespace OccultCrescentHelper;

public static class Helpers
{
    public unsafe static void DrawLine(Vector3 start, Vector3 end, float thickness, Vector4 color)
    {
        var camera = CameraManager.Instance()->CurrentCamera;
        var renderCamera = camera->RenderCamera;

        Matrix4x4 view = camera->ViewMatrix;
        Matrix4x4 projection = renderCamera->ProjectionMatrix;
        float nearPlane = renderCamera->NearPlane;
        uint width = Device.Instance()->Width;
        uint height = Device.Instance()->Height;

        if (!CameraHelper.WorldLineToScreen(start, end, view, projection, nearPlane, width, height, out Vector2 screenStart, out Vector2 screenEnd))
            return;

        uint imguiColor = ImGui.ColorConvertFloat4ToU32(color);
        ImGui.GetBackgroundDrawList().AddLine(screenStart, screenEnd, imguiColor, thickness);
    }
}
