namespace OccultCrescentHelper.Trackers;

public class PhantomJobExpTracker : ITracker
{
    private Tracked JobExp;

    public PhantomJobExpTracker()
    {
        // Silver = new Tracked("Silver per hour", GetSilverCount());
        // Gold = new Tracked("Gold per hour", GetGoldCount());
    }

    public double GetPhantomJobExp()
    {
        // Lumina.Excel.ExcelSheet<MKDSupportJob>();
        // Localchar
        // Svc.ClientState.LocalPlayer.

        // MkdInfo
        return 12;
    }

    public Tracked[] GetData()
    {
        return [JobExp];
    }

    public void Tick()
    {
        JobExp.UpdateValue(0);
    }
}
