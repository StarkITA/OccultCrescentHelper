using System;
using ECommons.DalamudServices;
using OccultCrescentHelper.Modules;

namespace OccultCrescentHelper.JobSwitcher;

public class JobSwitcherModule : Module, IDisposable
{
    public readonly JobSwitcher switcher;

    public JobSwitcherConfig config
    {
        get => _config.JobSwitcherConfig;
    }

    public JobSwitcherModule(Plugin plugin)
        : base(plugin)
    {
        switcher = new JobSwitcher(plugin);

        plugin.OnUpdate += switcher.Tick;
        Svc.Chat.ChatMessage += switcher.OnChatMessage;
    }

    public void Dispose()
    {
        plugin.OnUpdate -= switcher.Tick;
        Svc.Chat.ChatMessage -= switcher.OnChatMessage;
    }
}
