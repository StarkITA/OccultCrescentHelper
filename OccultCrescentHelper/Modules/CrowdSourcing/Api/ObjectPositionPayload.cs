namespace OccultCrescentHelper.Modules.CrowdSourcing.Api;

public struct ObjectPositionPayload
{
    public ObjectType type { get; set; }
    public Position position { get; set; }
    public uint? model_id { get; set; }
}
