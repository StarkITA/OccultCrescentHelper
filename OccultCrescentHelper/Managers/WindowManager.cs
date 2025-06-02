using System;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using OccultCrescentHelper.Windows;

namespace OccultCrescentHelper.Managers;

public class WindowManager : IDisposable
{
    private readonly WindowSystem windows = new("OccultCrescentHelper");

    private ConfigWindow config { get; init; }

    private MainWindow main { get; init; }

    public WindowManager(Plugin plugin)
    {
        config = new ConfigWindow(plugin);
        windows.AddWindow(config);

        main = new MainWindow(plugin);
        windows.AddWindow(main);

        Svc.PluginInterface.UiBuilder.Draw += DrawUI;
        Svc.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
        Svc.PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
    }

    private void DrawUI() => windows.Draw();

    public void ToggleConfigUI() => config.Toggle();

    public void ToggleMainUI() => main.Toggle();

    public void Dispose()
    {
        Svc.PluginInterface.UiBuilder.Draw -= DrawUI;
        Svc.PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUI;
        Svc.PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUI;

        windows.RemoveAllWindows();

        config.Dispose();
        main.Dispose();
    }
}
