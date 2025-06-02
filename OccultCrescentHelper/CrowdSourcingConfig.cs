using System;
using System.Collections.Generic;
using OccultCrescentHelper.Api;
using OccultCrescentHelper.ConfigAttributes;
using OccultCrescentHelper.Modules;

[Serializable]
[Title("Crowd Sourcing Config")]
public class CrowdSourcingConfig : ModuleConfig
{
    [CheckboxConfig]
    [Label("Share object position data")]
    [Tooltip("Send the positions of detected coffers and carrots to the crowdsourcing api.")]
    public bool ShareObjectPositionData { get; set; } = true;

    public List<ObjectPositionPayload> SharedObjectPosition { get; set; } = [];

    // public bool ShareMonsterPositionData { get; set; } = true;

    // public List<MonsterPayload> SharedMonsterLocations { get; set; } = [];
}
