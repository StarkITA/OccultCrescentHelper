namespace OccultCrescentHelper.Trackers;

public interface ITracker
{
    Tracked[] GetData();

    void Tick();
}
