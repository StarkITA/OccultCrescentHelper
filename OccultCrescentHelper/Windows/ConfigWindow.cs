using System;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace OccultCrescentHelper.Windows;

public class ConfigWindow : Window, IDisposable
{
    public ConfigWindow(Plugin plugin)
        : base(plugin.Name + " Config")
    {
        Flags =
            ImGuiWindowFlags.NoResize
            | ImGuiWindowFlags.NoCollapse
            | ImGuiWindowFlags.NoScrollbar
            | ImGuiWindowFlags.NoScrollWithMouse;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.TextUnformatted(":D");
    }
}
