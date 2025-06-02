using System;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using ImGuiNET;
using Lumina.Excel.Sheets;
using OccultCrescentHelper.Api;

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
