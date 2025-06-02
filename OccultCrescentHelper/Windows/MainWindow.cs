using System;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace OccultCrescentHelper.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin plugin;

    public MainWindow(Plugin plugin)
        : base(plugin.Name, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        plugin.treasures.Draw();
        plugin.carrots.Draw();
        plugin.currency.Draw();
        plugin.fates.Draw();
    }
}
