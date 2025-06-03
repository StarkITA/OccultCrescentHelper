using System;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
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

    public override bool enabled
    {
        get => config.Enabled;
    }

    public JobSwitcherModule(Plugin plugin)
        : base(plugin)
    {
        switcher = new JobSwitcher(plugin);

        plugin.OnUpdate += Tick;
        Svc.Chat.ChatMessage += OnChatMessage;
    }

    public void Tick(IFramework framework)
    {
        if (!enabled || !Helpers.IsInOccultCrescent())
        {
            return;
        }

        switcher.Tick(framework);
    }

    public void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!enabled || !Helpers.IsInOccultCrescent())
        {
            return;
        }

        switcher.OnChatMessage(type, timestamp, ref sender, ref message, ref isHandled);
    }

    public void Dispose()
    {
        plugin.OnUpdate -= Tick;
        Svc.Chat.ChatMessage -= OnChatMessage;
    }
}
