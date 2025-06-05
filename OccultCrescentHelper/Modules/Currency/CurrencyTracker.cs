using System;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace OccultCrescentHelper.Modules.Currency;

public class CurrencyTracker
{
    private enum Currency
    {
        Silver = 45043,
        Gold = 45044,
    }

    private float startingGold = 0f;

    private float currentGold = 0f;

    private DateTime goldStartTime = DateTime.UtcNow;

    private float startingSilver = 0f;

    private float currentSilver = 0f;

    private DateTime silverStartTime = DateTime.UtcNow;

    public CurrencyTracker()
    {
        Reset();
    }

    public void Tick(IFramework _)
    {
        currentGold = GetGold();
        currentSilver = GetSilver();
    }

    public void TerritoryChanged(ushort _) => Reset();

    public void ResetSilver()
    {
        startingSilver = GetSilver();
        currentSilver = startingSilver;
        silverStartTime = DateTime.UtcNow;
    }

    public void ResetGold()
    {
        startingGold = GetGold();
        currentGold = startingGold;
        goldStartTime = DateTime.UtcNow;
    }

    public void Reset()
    {
        ResetSilver();
        ResetGold();
    }

    public float GetGoldPerHour()
    {
        var elapsed = (float)(DateTime.UtcNow - goldStartTime).TotalHours;
        if (elapsed <= 0)
            return 0;

        return (currentGold - startingGold) / elapsed;
    }

    public float GetSilverPerHour()
    {
        var elapsed = (float)(DateTime.UtcNow - silverStartTime).TotalHours;
        if (elapsed <= 0)
            return 0;

        return (currentSilver - startingSilver) / elapsed;
    }

    private unsafe float GetGold() => InventoryManager.Instance()->GetInventoryItemCount((uint)Currency.Gold);

    private unsafe float GetSilver() => InventoryManager.Instance()->GetInventoryItemCount((uint)Currency.Silver);
}
