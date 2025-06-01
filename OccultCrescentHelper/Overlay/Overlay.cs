using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ImGuiNET;

namespace OccultCrescentHelper.Overlay;

public class Overlay : Window, IDisposable
{
    private List<IOverlayChild> children = [];

    public readonly Plugin plugin;

    public Overlay(Plugin plugin)
        : base("Overlay##OCH", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.plugin = plugin;

        children.Add(new TreasureOverlay());
        children.Add(new CarrotOverlay());
    }

    public void Dispose() { }

    public override void Draw()
    {
        if (!Helpers.IsInOccultCrescent())
        {
            return;
        }

        foreach (var child in children)
        {
            child.Draw(this);
        }
    }

    public void DrawLine(Vector3 start, Vector3 end, float thickness, Vector4 color)
    {
        bool startValid = Svc.GameGui.WorldToScreen(start, out Vector2 startScreen);
        bool endValid = Svc.GameGui.WorldToScreen(end, out Vector2 endScreen);

        if (startValid && endValid)
        {
            var imguiColor = ImGui.ColorConvertFloat4ToU32(color);
            ImGui.GetBackgroundDrawList().AddLine(startScreen, endScreen, imguiColor, thickness);
        }
    }
}
