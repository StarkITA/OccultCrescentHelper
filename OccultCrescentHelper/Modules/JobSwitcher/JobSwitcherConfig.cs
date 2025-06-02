using System;
using OccultCrescentHelper.ConfigAttributes;
using OccultCrescentHelper.Modules;

namespace OccultCrescentHelper.JobSwitcher;

[Serializable]
[Title("Job Switcher Config")]
public class JobSwitcherConfig : ModuleConfig
{
    [CheckboxConfig]
    [Label("Switch Job when combat is over")]
    [Tooltip("This works for fates and mob farming.\nThis allows you to get exp in a different Phantom job than the one you fought with.")]
    public bool SwitchJobOnCombatEnd { get; set; } = false;

    [PhantomJobConfig]
    [RenderIf(nameof(SwitchJobOnCombatEnd))]
    [Label("Combat Job")]
    [Tooltip("Job to switch to for Fates/Combat.\nCritical engagements too depending on the value of [Switch to experience job beore Critical Encounter].")]
    public uint CombatJob { get; set; } = 1;

    [PhantomJobConfig]
    [RenderIf(nameof(SwitchJobOnCombatEnd))]
    [Label("Experience Job")]
    [Tooltip("Job to switch to after Fates/Combat.\nCritical engagements too depending on the value of [Switch to experience job beore Critical Encounter].")]
    public uint ExpJob { get; set; } = 1;

    [CheckboxConfig]
    [RenderIf(nameof(SwitchJobOnCombatEnd))]
    [Label("Switch to experience job beore Critical Encounter")]
    [Tooltip("Since you cannot switch to a job before getting experience from a ce, we switch before hand.")]
    public bool SwitchToExpJobOnCE { get; set; } = true;

    [CheckboxConfig]
    [ExperimentalFeature]
    [RenderIf(nameof(SwitchJobOnCombatEnd))]
    [Label("Return after finishing a Fate")]
    [Tooltip("Cast Occult Return after finishing a Fate (wip.)")]
    public bool ReturnAfterFate { get; set; } = false;

    [CheckboxConfig]
    [ExperimentalFeature]
    [RenderIf(nameof(SwitchJobOnCombatEnd))]
    [Label("Return after finishing a Critical Encounter")]
    [Tooltip("Cast Occult Return after finishing a Critical Encounter (wip.)")]
    public bool ReturnAfterCE { get; set; } = false;
}
