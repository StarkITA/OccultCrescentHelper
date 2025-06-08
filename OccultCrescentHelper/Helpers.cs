using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
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

    public static bool IsInForkedTower()
    {
        var player = Svc.ClientState.LocalPlayer;
        if (player == null)
        {
            return false;
        }

        List<uint> forkedTowerStatuses = [4262, 4263, 4228];
        return player.StatusList.Any(s => forkedTowerStatuses.Contains(s.StatusId));
    }

    public static void KDrawLine(Vector3 start, Vector3 end, float thickness, Vector4 color)
    {
        bool startValid = Svc.GameGui.WorldToScreen(start, out Vector2 startScreen);
        bool endValid = Svc.GameGui.WorldToScreen(end, out Vector2 endScreen);

        if (startValid && endValid)
        {
            var imguiColor = ImGui.ColorConvertFloat4ToU32(color);
            ImGui.GetBackgroundDrawList().AddLine(startScreen, endScreen, imguiColor, thickness);
        }
    }

    public unsafe static void DrawLine(Vector3 start, Vector3 end, float thickness, Vector4 color)
    {
        var camera = FFXIVClientStructs.FFXIV.Client.Graphics.Scene.CameraManager.Instance()->CurrentCamera;
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
