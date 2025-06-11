using System.Collections.Generic;
using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.Automator;

[Title("Illegal Mode")]
public partial class AutomatorConfig : ModuleConfig
{
    public override string ProviderNamespace => "OccultCrescentHelper.Modules.Automator";

    [Checkbox]
    [Illegal]
    [Experimental]
    [RequiredPlugin("Lifestream", "vnavmesh")]
    [Label("Enabled")]
    public bool Enabled { get; set; } = false;

    [Enum(typeof(AiType), nameof(AiTypeProvider))]
    [DependsOn(nameof(Enabled))]
    [Label("AI provider")]
    [Tooltip("The mechanic AI provider you use")]
    public AiType AiProvider { get; set; } = AiType.VBM;

    [Checkbox]
    [DependsOn(nameof(Enabled))]
    [Label("Toggle Ai Provider")]
    [Tooltip("Toggle Ai Provider on when starting fate/ce\nToggle Ai Provider off when navigating to fate/ce")]
    public bool ToggleAiProvider { get; set; } = true;
    public bool ShouldToggleAiProvider => IsPropertyEnabled(nameof(ToggleAiProvider));

    [Checkbox]
    [DependsOn(nameof(Enabled))]
    [Label("Force Target")]
    [Tooltip("Ensure you always maintain a target in fates and critical encounters.")]
    public bool ForceTarget { get; set; } = true;
    public bool ShouldForceTarget => IsPropertyEnabled(nameof(ForceTarget));

    [FloatRange(5f, 30f)]
    [DependsOn(nameof(Enabled))]
    [Label("Engagement Range")]
    [Tooltip("The range to be at from a fate monster before dismounting and letting your rotation/ai plugin take over")]
    public float EngagementRange { get; set; } = 5f;

    // Critical Encounters
    [Checkbox]
    [DependsOn(nameof(Enabled))]
    [Label("Do Critical Encounters")]
    [Tooltip("Should consider critical encounters when choosing an activity")]
    public bool DoCriticalEncounters { get; set; } = true;
    public bool ShouldDoCriticalEncounters => IsPropertyEnabled(nameof(DoCriticalEncounters));

    // Critical Encounters
    [Checkbox]
    [DependsOn(nameof(Enabled), nameof(DoCriticalEncounters))]
    [Label("Delay before heading to Critical Encoutner")]
    [Tooltip("Add a 10 - 15 second delay before heading to a critical encoutner")]
    public bool DelayCriticalEncounters { get; set; } = false;
    public bool ShouldDelayCriticalEncounters => IsPropertyEnabled(nameof(DelayCriticalEncounters));

    [Checkbox]
    [DependsOn(nameof(Enabled), nameof(DoCriticalEncounters))]
    [Indent]
    [Label("CE: Scourge of the Mind")]
    [Tooltip("Toggle participation in the 'Scourge of the Mind' critical encounter.")]
    public bool DoScourgeOfTheMind { get; set; } = true;
    public bool ShouldDoScourgeOfTheMind => IsPropertyEnabled(nameof(DoScourgeOfTheMind));

    [Checkbox]
    [DependsOn(nameof(Enabled), nameof(DoCriticalEncounters))]
    [Indent]
    [Label("CE: The Black Regiment")]
    [Tooltip("Toggle participation in the 'The Black Regiment' critical encounter.")]
    public bool DoTheBlackRegiment { get; set; } = true;
    public bool ShouldDoTheBlackRegiment => IsPropertyEnabled(nameof(DoTheBlackRegiment));

    [Checkbox]
    [DependsOn(nameof(Enabled), nameof(DoCriticalEncounters))]
    [Indent]
    [Label("CE: The Unbridled")]
    [Tooltip("Toggle participation in the 'The Unbridled' critical encounter.")]
    public bool DoTheUnbridled { get; set; } = true;
    public bool ShouldDoTheUnbridled => IsPropertyEnabled(nameof(DoTheUnbridled));

    [Checkbox]
    [DependsOn(nameof(Enabled), nameof(DoCriticalEncounters))]
    [Indent]
    [Label("CE: Crawling Death")]
    [Tooltip("Toggle participation in the 'Crawling Death' critical encounter.")]
    public bool DoCrawlingDeath { get; set; } = true;
    public bool ShouldDoCrawlingDeath => IsPropertyEnabled(nameof(DoCrawlingDeath));

    [Checkbox]
    [DependsOn(nameof(Enabled), nameof(DoCriticalEncounters))]
    [Indent]
    [Label("CE: Calamity Bound")]
    [Tooltip("Toggle participation in the 'Calamity Bound' critical encounter.")]
    public bool DoCalamityBound { get; set; } = true;
    public bool ShouldDoCalamityBound => IsPropertyEnabled(nameof(DoCalamityBound));

    [Checkbox]
    [DependsOn(nameof(Enabled), nameof(DoCriticalEncounters))]
    [Indent]
    [Label("CE: Trial by Claw")]
    [Tooltip("Toggle participation in the 'Trial by Claw' critical encounter.")]
    public bool DoTrialByClaw { get; set; } = true;
    public bool ShouldDoTrialByClaw => IsPropertyEnabled(nameof(DoTrialByClaw));

    [Checkbox]
    [DependsOn(nameof(Enabled), nameof(DoCriticalEncounters))]
    [Indent]
    [Label("CE: From Times Bygone")]
    [Tooltip("Toggle participation in the 'From Times Bygone' critical encounter.")]
    public bool DoFromTimesBygone { get; set; } = true;
    public bool ShouldDoFromTimesBygone => IsPropertyEnabled(nameof(DoFromTimesBygone));

    [Checkbox]
    [DependsOn(nameof(Enabled), nameof(DoCriticalEncounters))]
    [Indent]
    [Label("CE: Company of Stone")]
    [Tooltip("Toggle participation in the 'Company of Stone' critical encounter.")]
    public bool DoCompanyOfStone { get; set; } = true;
    public bool ShouldDoCompanyOfStone => IsPropertyEnabled(nameof(DoCompanyOfStone));

    [Checkbox]
    [DependsOn(nameof(Enabled), nameof(DoCriticalEncounters))]
    [Indent]
    [Label("CE: Shark Attack")]
    [Tooltip("Toggle participation in the 'Shark Attack' critical encounter.")]
    public bool DoSharkAttack { get; set; } = true;
    public bool ShouldDoSharkAttack => IsPropertyEnabled(nameof(DoSharkAttack));

    [Checkbox]
    [Indent]
    [DependsOn(nameof(Enabled), nameof(DoCriticalEncounters))]
    [Label("CE: On the Hunt")]
    [Tooltip("Toggle participation in the 'On the Hunt' critical encounter.")]
    public bool DoOnTheHunt { get; set; } = true;
    public bool ShouldDoOnTheHunt => IsPropertyEnabled(nameof(DoOnTheHunt));

    [Checkbox]
    [DependsOn(nameof(Enabled), nameof(DoCriticalEncounters))]
    [Indent]
    [Label("CE: With Extreme Prejudice")]
    [Tooltip("Toggle participation in the 'With Extreme Prejudice' critical encounter.")]
    public bool DoWithExtremePrejudice { get; set; } = true;
    public bool ShouldDoWithExtremePrejudice => IsPropertyEnabled(nameof(DoWithExtremePrejudice));

    [Checkbox]
    [DependsOn(nameof(Enabled), nameof(DoCriticalEncounters))]
    [Indent]
    [Label("CE: Noise Complaint")]
    [Tooltip("Toggle participation in the 'Noise Complaint' critical encounter.")]
    public bool DoNoiseComplaint { get; set; } = true;
    public bool ShouldDoNoiseComplaint => IsPropertyEnabled(nameof(DoNoiseComplaint));

    [Checkbox]
    [DependsOn(nameof(Enabled), nameof(DoCriticalEncounters))]
    [Indent]
    [Label("CE: Cursed Concern")]
    [Tooltip("Toggle participation in the 'Cursed Concern' critical encounter.")]
    public bool DoCursedConcern { get; set; } = true;
    public bool ShouldDoCursedConcern => IsPropertyEnabled(nameof(DoCursedConcern));

    [Checkbox]
    [DependsOn(nameof(Enabled), nameof(DoCriticalEncounters))]
    [Indent]
    [Label("CE: Eternal Watch")]
    [Tooltip("Toggle participation in the 'Eternal Watch' critical encounter.")]
    public bool DoEternalWatch { get; set; } = true;
    public bool ShouldDoEternalWatch => IsPropertyEnabled(nameof(DoEternalWatch));

    [Checkbox]
    [DependsOn(nameof(Enabled), nameof(DoCriticalEncounters))]
    [Indent]
    [Label("CE: Flame of Dusk")]
    [Tooltip("Toggle participation in the 'Flame of Dusk' critical encounter.")]
    public bool DoFlameOfDusk { get; set; } = true;
    public bool ShouldDoFlameOfDusk => IsPropertyEnabled(nameof(DoFlameOfDusk));

    // Fates
    [Checkbox]
    [DependsOn(nameof(Enabled))]
    [Label("Do Fates")]
    [Tooltip("Should consider fates when choosing an activity")]
    public bool DoFates { get; set; } = true;
    public bool ShouldDoFates => IsPropertyEnabled(nameof(DoFates));

    [Checkbox]
    [Indent]
    [DependsOn(nameof(Enabled), nameof(DoFates))]
    [Label("Fate: Rough Waters")]
    [Tooltip("Toggle participation in the 'Rough Waters' fate.")]
    public bool DoRoughWaters { get; set; } = true;
    public bool ShouldDoRoughWaters => IsPropertyEnabled(nameof(DoRoughWaters));

    [Checkbox]
    [Indent]
    [DependsOn(nameof(Enabled), nameof(DoFates))]
    [Label("Fate: The Golden Guardian")]
    [Tooltip("Toggle participation in the 'The Golden Guardian' fate.")]
    public bool DoTheGoldenGuardian { get; set; } = true;
    public bool ShouldDoTheGoldenGuardian => IsPropertyEnabled(nameof(DoTheGoldenGuardian));

    [Checkbox]
    [Indent]
    [DependsOn(nameof(Enabled), nameof(DoFates))]
    [Label("Fate: King of the Crescent")]
    [Tooltip("Toggle participation in the 'King of the Crescent' fate.")]
    public bool DoKingOfTheCrescent { get; set; } = true;
    public bool ShouldDoKingOfTheCrescent => IsPropertyEnabled(nameof(DoKingOfTheCrescent));

    [Checkbox]
    [Indent]
    [DependsOn(nameof(Enabled), nameof(DoFates))]
    [Label("Fate: The Winged Terror")]
    [Tooltip("Toggle participation in the 'The Winged Terror' fate.")]
    public bool DoTheWingedTerror { get; set; } = true;
    public bool ShouldDoTheWingedTerror => IsPropertyEnabled(nameof(DoTheWingedTerror));

    [Checkbox]
    [Indent]
    [DependsOn(nameof(Enabled), nameof(DoFates))]
    [Label("Fate: An Unending Duty")]
    [Tooltip("Toggle participation in the 'An Unending Duty' fate.")]
    public bool DoAnUnendingDuty { get; set; } = true;
    public bool ShouldDoAnUnendingDuty => IsPropertyEnabled(nameof(DoAnUnendingDuty));

    [Checkbox]
    [Indent]
    [DependsOn(nameof(Enabled), nameof(DoFates))]
    [Label("Fate: Brain Drain")]
    [Tooltip("Toggle participation in the 'Brain Drain' fate.")]
    public bool DoBrainDrain { get; set; } = true;
    public bool ShouldDoBrainDrain => IsPropertyEnabled(nameof(DoBrainDrain));

    [Checkbox]
    [Indent]
    [DependsOn(nameof(Enabled), nameof(DoFates))]
    [Label("Fate: A Delicate Balance")]
    [Tooltip("Toggle participation in the 'A Delicate Balance' fate.")]
    public bool DoADelicateBalance { get; set; } = true;
    public bool ShouldDoADelicateBalance => IsPropertyEnabled(nameof(DoADelicateBalance));

    [Checkbox]
    [Indent]
    [DependsOn(nameof(Enabled), nameof(DoFates))]
    [Label("Fate: Sworn to Soil")]
    [Tooltip("Toggle participation in the 'Sworn to Soil' fate.")]
    public bool DoSwornToSoil { get; set; } = true;
    public bool ShouldDoSwornToSoil => IsPropertyEnabled(nameof(DoSwornToSoil));

    [Checkbox]
    [Indent]
    [DependsOn(nameof(Enabled), nameof(DoFates))]
    [Label("Fate: A Prying Eye")]
    [Tooltip("Toggle participation in the 'A Prying Eye' fate.")]
    public bool DoAPryingEye { get; set; } = true;
    public bool ShouldDoAPryingEye => IsPropertyEnabled(nameof(DoAPryingEye));

    [Checkbox]
    [Indent]
    [DependsOn(nameof(Enabled), nameof(DoFates))]
    [Label("Fate: Fatal Allure")]
    [Tooltip("Toggle participation in the 'Fatal Allure' fate.")]
    public bool DoFatalAllure { get; set; } = true;
    public bool ShouldDoFatalAllure => IsPropertyEnabled(nameof(DoFatalAllure));

    [Checkbox]
    [Indent]
    [DependsOn(nameof(Enabled), nameof(DoFates))]
    [Label("Fate: Serving Darkness")]
    [Tooltip("Toggle participation in the 'Serving Darkness' fate.")]
    public bool DoServingDarkness { get; set; } = true;
    public bool ShouldDoServingDarkness => IsPropertyEnabled(nameof(DoServingDarkness));

    [Checkbox]
    [Experimental]
    [Indent]
    [DependsOn(nameof(Enabled), nameof(DoFates))]
    [Label("Fate: Persistent Pots")]
    [Tooltip("Toggle participation in the 'Persistent Pots' fate.")]
    public bool DoPersistentPots { get; set; } = false;
    public bool ShouldDoPersistentPots => IsPropertyEnabled(nameof(DoPersistentPots));

    [Checkbox]
    [Experimental]
    [Indent]
    [DependsOn(nameof(Enabled), nameof(DoFates))]
    [Label("Fate: Pleading Pots")]
    [Tooltip("Toggle participation in the 'Pleading Pots' fate.")]
    public bool DoPleadingPots { get; set; } = false;
    public bool ShouldDoPleadingPots => IsPropertyEnabled(nameof(DoPleadingPots));

    public IReadOnlyDictionary<uint, bool> CriticalEncountersMap => new Dictionary<uint, bool>
    {
        { 1, ShouldDoScourgeOfTheMind },
        { 2, ShouldDoTheBlackRegiment },
        { 3, ShouldDoTheUnbridled },
        { 4, ShouldDoCrawlingDeath },
        { 5, ShouldDoCalamityBound },
        { 6, ShouldDoTrialByClaw },
        { 7, ShouldDoFromTimesBygone },
        { 8, ShouldDoCompanyOfStone },
        { 9, ShouldDoSharkAttack },
        { 10, ShouldDoOnTheHunt },
        { 11, ShouldDoWithExtremePrejudice },
        { 12, ShouldDoNoiseComplaint },
        { 13, ShouldDoCursedConcern },
        { 14, ShouldDoEternalWatch },
        { 15, ShouldDoFlameOfDusk },
    };

    public IReadOnlyDictionary<uint, bool> FatesMap => new Dictionary<uint, bool>
    {
        { 1962, ShouldDoRoughWaters },
        { 1963, ShouldDoTheGoldenGuardian },
        { 1964, ShouldDoKingOfTheCrescent },
        { 1965, ShouldDoTheWingedTerror },
        { 1966, ShouldDoAnUnendingDuty },
        { 1967, ShouldDoBrainDrain },
        { 1968, ShouldDoADelicateBalance },
        { 1969, ShouldDoSwornToSoil },
        { 1970, ShouldDoAPryingEye },
        { 1971, ShouldDoFatalAllure },
        { 1972, ShouldDoServingDarkness },
        { 1976, ShouldDoPersistentPots },
        { 1977, ShouldDoPleadingPots },
    };
}
