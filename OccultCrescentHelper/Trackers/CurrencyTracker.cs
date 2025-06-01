using FFXIVClientStructs.FFXIV.Client.Game;

namespace OccultCrescentHelper.Trackers;

public class CurrencyTracker : ITracker
{
    private enum Currency
    {
        Silver = 45043,
        Gold = 45044,
    }

    private Tracked Silver;

    private Tracked Gold;

    public CurrencyTracker()
    {
        Silver = new Tracked("Silver per hour", GetSilverCount());
        Gold = new Tracked("Gold per hour", GetGoldCount());
    }

    public unsafe double GetSilverCount()
    {
        return InventoryManager.Instance()->GetInventoryItemCount((uint)Currency.Silver);
    }

    public unsafe double GetGoldCount()
    {
        return InventoryManager.Instance()->GetInventoryItemCount((uint)Currency.Gold);
    }

    public Tracked[] GetData()
    {
        return [Silver, Gold];
    }

    public void Tick()
    {
        Silver.UpdateValue(GetSilverCount());
        Gold.UpdateValue(GetGoldCount());
    }
}
