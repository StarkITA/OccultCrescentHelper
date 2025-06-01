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
        : base("Overlay##OVH", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.plugin = plugin;

        children.Add(new TreasureOverlay());
        children.Add(new CarrotOverlay());
    }

    public void Dispose() { }

    public override void Draw()
    {
        if (Start())
        {
            foreach (var child in children)
            {
                child.Draw(this);
            }

            End();
        }
    }

    public bool Start()
    {
        if (Svc.ClientState.LocalPlayer == null)
        {
            return false;
        }

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, Vector2.Zero);

        // Ensure the window background is fully transparent
        ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0, 0, 0, 0));
        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0, 0, 0, 0));

        ImGuiWindowFlags windowFlags =
            ImGuiWindowFlags.NoInputs
            | ImGuiWindowFlags.NoTitleBar
            | ImGuiWindowFlags.NoMove
            | ImGuiWindowFlags.NoResize
            | ImGuiWindowFlags.NoScrollbar
            | ImGuiWindowFlags.NoScrollWithMouse
            | ImGuiWindowFlags.NoCollapse
            | ImGuiWindowFlags.NoBackground
            | ImGuiWindowFlags.NoSavedSettings
            | ImGuiWindowFlags.NoBringToFrontOnFocus
            | ImGuiWindowFlags.NoDocking;

        ImGui.SetNextWindowPos(Vector2.Zero);
        ImGui.SetNextWindowSize(ImGui.GetIO().DisplaySize);

        // Use unique window ID to avoid conflicts with other plugins
        bool opened = ImGui.Begin("DrawingOverlay##OCH", windowFlags);
        if (opened)
        {
            return true;
        }

        // Clean up if window creation failed
        ImGui.PopStyleColor(2);
        ImGui.PopStyleVar(3);
        return false;
    }

    private void End()
    {
        ImGui.End();
        ImGui.PopStyleColor(2);
        ImGui.PopStyleVar(3);
    }

    /// Thanks Wah
    public void DrawLine(Vector3 start, Vector3 end, float thickness, Vector4 color)
    {
        bool startValid = Svc.GameGui.WorldToScreen(start, out Vector2 startScreen);
        bool endValid = Svc.GameGui.WorldToScreen(end, out Vector2 endScreen);

        if (startValid && endValid)
        {
            var imguiColor = ImGui.ColorConvertFloat4ToU32(color);
            ImGui.GetWindowDrawList().AddLine(startScreen, endScreen, imguiColor, thickness);
        }
    }
}
