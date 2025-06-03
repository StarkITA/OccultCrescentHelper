using System;
using OccultCrescentHelper.ConfigAttributes;
using OccultCrescentHelper.Modules;

namespace OccultCrescentHelper.JobSwitcher;

[Serializable]
[Title("Content Handler Config")]
public class JobSwitcherConfig : ModuleConfig
{
    [CheckboxConfig]
    [Label("Enabled")]
    public bool Enabled { get; set; } = true;

    [CheckboxConfig]
    [RenderIf(nameof(Enabled))]
    [Label("Show State Debug")]
    public bool ShowStateDebug { get; set; } = false;

    [CheckboxConfig]
    [RenderIf(nameof(Enabled))]
    [Label("Switch Job when combat is over")]
    [Tooltip("This works for fates and mob farming.\nThis allows you to get exp in a different Phantom job than the one you fought with.")]
    public bool SwitchJobOnCombatEnd { get; set; } = false;

    [PhantomJobConfig]
    [RenderIf(nameof(Enabled), nameof(SwitchJobOnCombatEnd))]
    [Label("Combat Job")]
    [Tooltip("Job to switch to for Fates/Combat.\nCritical encounters too depending on the value of [Switch to experience job before Critical Encounter].")]
    public uint CombatJob { get; set; } = 1;

    [PhantomJobConfig]
    [RenderIf(nameof(Enabled), nameof(SwitchJobOnCombatEnd))]
    [Label("Experience Job")]
    [Tooltip("Job to switch to after Fates/Combat.\nCritical encounters too depending on the value of [Switch to experience job before Critical Encounter].")]
    public uint ExpJob { get; set; } = 1;

    [CheckboxConfig]
    [RenderIf(nameof(Enabled), nameof(SwitchJobOnCombatEnd))]
    [Label("Switch to experience job before Critical Encounter")]
    [Tooltip("Since you cannot switch to a job before getting experience from a ce, we switch before hand.")]
    public bool SwitchToExpJobOnCE { get; set; } = true;

    [CheckboxConfig]
    [ExperimentalFeature]
    [RenderIf(nameof(Enabled))]
    [Label("Return after finishing a Fate")]
    [Tooltip("Cast Occult Return after finishing a Fate (wip.)")]
    public bool ReturnAfterFate { get; set; } = false;

    [CheckboxConfig]
    [ExperimentalFeature]
    [RenderIf(nameof(Enabled))]
    [Label("Return after finishing a Critical Encounter")]
    [Tooltip("Cast Occult Return after finishing a Critical Encounter (wip.)")]
    public bool ReturnAfterCE { get; set; } = false;

    [CheckboxConfig]
    [ExperimentalFeature]
    [RenderIf(nameof(Enabled))]
    [Label("Walk to the Aetheryte in camp after returning")]
    [Tooltip("Walk to the Aetheryte in camp after returning, ready to teleport to the next event.")]
    public bool ApproachAetheryteAfterReturn { get; set; } = false;
}
