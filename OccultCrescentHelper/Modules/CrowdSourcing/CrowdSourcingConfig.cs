using System.Collections.Generic;
using OccultCrescentHelper.Modules.CrowdSourcing.Api;
using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.CrowdSourcing;

[Title("Crowd Sourcing Config")]
public class CrowdSourcingConfig : ModuleConfig
{
    [Checkbox]
    [Label("Share object position data")]
    [Tooltip("Send the positions of detected coffers and carrots to the crowdsourcing api.")]
    public bool ShareObjectPositionData { get; set; } = false;

    public List<ObjectPositionPayload> SharedObjectPosition { get; set; } = [];
}
