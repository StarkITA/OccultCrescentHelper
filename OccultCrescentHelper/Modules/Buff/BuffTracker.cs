using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using FFXIVClientStructs.FFXIV.Common.Math;
using OccultCrescentHelper.Data;
using OccultCrescentHelper.Enums;
using Ocelot.Chain;
using Ocelot.Chain.ChainEx;

namespace OccultCrescentHelper.Modules.Buff;

public class BuffTracker
{
    public BuffTracker()
    {
        Reset();
    }

    public void Tick(IFramework _) { }

    public void Reset() { }

    public bool IsNearCrystal()
    {
        return NearbyCrystalCount() > 0;
    }

    public int NearbyCrystalCount()
    {
        var playerPos = Svc.ClientState.LocalPlayer?.Position ?? Vector3.Zero;

        return Svc.Objects
                  .Where(o => o.ObjectKind == ObjectKind.EventObj)
                  .Where(o => o.DataId == 2007457)
                  .Count(o => Vector3.Distance(o.Position, playerPos) <= 4.5f);
    }
    

    private Chain BardChain()
    {
        return Chain.Create("Buffs:Bard")
                    .Then(_ => PublicContentOccultCrescent.ChangeSupportJob((byte)JobId.Bard))
                    .WaitUntilStatus((uint)PlayerStatus.PhantomBard)
                    .WaitGcd()
                    .UseAction(ActionType.GeneralAction, 32)
                    .WaitUntilStatus((uint)PlayerStatus.RomeosBallad)
                    .WaitGcd();
    }

    private Chain MonkChain()
    {
        return Chain.Create("Buffs:Monk")
                    .Then(_ => PublicContentOccultCrescent.ChangeSupportJob((byte)JobId.Monk))
                    .WaitUntilStatus((uint)PlayerStatus.PhantomMonk)
                    .WaitGcd()
                    .UseAction(ActionType.GeneralAction, 33)
                    .WaitUntilStatus((uint)PlayerStatus.Fleetfooted)
                    .WaitGcd();
    }

    private Chain KnightChain()
    {
        return Chain.Create("Buffs:Knight")
                    .Then(_ => PublicContentOccultCrescent.ChangeSupportJob((byte)JobId.Knight))
                    .WaitUntilStatus((uint)PlayerStatus.PhantomKnight)
                    .WaitGcd()
                    .UseAction(ActionType.GeneralAction, 32)
                    .WaitUntilStatus((uint)PlayerStatus.EnduringFortitude)
                    .WaitGcd();
    }

    private Chain BackToStartingJob(byte startingJobId)
    {
        return Chain.Create("Buffs:StartingJob")
                    .Then(_ => PublicContentOccultCrescent.ChangeSupportJob(startingJobId))
                    .WaitGcd();
    }

    public void SwitchJobAndBuff()
    {
        var startingJobId = StartingJobId();
        Svc.Log.Info($"Switching FROM {startingJobId}");
        var manager = ChainManager.Get("OCH##BuffManager");
        if (manager.IsRunning)
        {
            return;
        }


        Chain Factory()
        {
            return BardChain().Then(MonkChain).Then(KnightChain);
        }

        manager.Submit(Factory);
    }

    private unsafe byte StartingJobId()
    {
        return PublicContentOccultCrescent.GetState()->CurrentSupportJob;
    }
}
