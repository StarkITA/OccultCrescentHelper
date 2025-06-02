using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;

namespace OccultCrescentHelper.JobSwitcher;

public interface IJobStateHandler
{
    void Enter();

    void Tick(IFramework framework);

    void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled);
}
