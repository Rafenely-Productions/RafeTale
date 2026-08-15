namespace RafeTale.Domain.Enums;

[Flags]
public enum ConditionType
{
    None = 0,
    Blinded = 1 << 0,
    Charmed = 1 << 1,
    Deafened = 1 << 2,
    Frightened = 1 << 3,
    Grappled = 1 << 4,
    Incapacitated = 1 << 5,
    Invisible = 1 << 6,
    Paralyzed = 1 << 7,
    Petrified = 1 << 8,
    Poisoned = 1 << 9,
    Prone = 1 << 10,
    Restrained = 1 << 11,
    Stunned = 1 << 12,
    Unconscious = 1 << 13,
    Exhaustion = 1 << 14
}

public enum TargetPropertyType
{
    Strength,
    Dexterity,
    Constitution,
    Intelligence,
    Wisdom,
    Charisma,
    ArmorClass,
    Speed,
    Initiative,
    MaxHp,
    SavingThrowStrength,
    SavingThrowDexterity,
    SavingThrowConstitution,
    SavingThrowIntelligence,
    SavingThrowWisdom,
    SavingThrowCharisma,
    SpellSaveDc,
    SpellAttackBonus
}

public enum ModifierDurationType
{
    Instantaneous,
    UntilEndOfTurn,
    Rounds,
    UntilShortRest,
    UntilLongRest,
    Permanent
}

public enum ModifierType
{
    AttributeBonus,
    GrantSpell,
    GrantFeature,
    HpBonus,
    Special,
    GrantResource,
    ActiveAbility,
    InitiativeBonus,
    CombatAbility,
    DiceRule,
    HpBonusPerLevel,
    GrantProficiencyChoice,
    MarketDiscount,
    RestAbility,
    AnySkillOrTool,
    WeaponOverride,
    RequiredLevel,
    GrantProficiency,
    FeatProperty,
    None,

    SetBaseArmorClass,
    ModifyDamage,
    UnlockWeaponMastery,
    EnableOption,
    GrantAdvantage,
    GrantSubclass,
    GrantFeat,
    ModifyAttacksPerAttackAction,
    ModifySpeed,
    GrantSubclassFeature,
    ModifyActionEffect,
    AddDamageBonus,
    EnableDeathPrevention,
    ScaleResource,
    ExpandMightyBlowEffects,
    TriggerOnInitiative,
    SetMinimumRollValue,
    ModifyAbilityScoreMaximum,
    EnableSpellcasting,
    ModifyAbilityCheck,
    GrantExpertise,
    ModifyResourceReset,
    ReactionEffect,
    ExpandSpellLists,
    UpdatePreparedSpellsCount,
    InitiativeTrigger,
    PermanentSpellMemory,
    GrantArcaneSecrets,
    EnableBondedCasting,
    ShortRitualTrigger,
    ScaleBondedSlots,
    GrantArcanumSecret,
    EnhanceFeature,
    SelectChoice,
    SelectFeat,
    ModifyFeature,
    GrantLanguage,
    SelectOption,
    UpgradeFeature,
    PermanentSpellMemorySingle,
    GrantLanguages,
    ModifyAttacksPerAction,
    GrantSense,
    UnlockWeaponMasterySlots,
    GrantFeatCategory,
    ModifyAction,
    EmanationAura,
    Action,
    ModifyAura,
    OnHitModifier,
    BonusActionOptions,
    BonusAction,
    Reaction,
    UnlockAmbushOptions,
    PassiveEffect,
    GrantSaveProficiency,
    FeatureCooldown,
}

public enum AttributeImprovementChoice
{
    Strength,
    Dexterity,
    Constitution,
    Intelligence,
    Wisdom,
    Charisma
}

public enum DamageType
{
    Bludgeoning,
    Piercing,
    Slashing,
    Fire,
    Cold,
    Lightning,
    Thunder,
    Acid,
    Poison,
    Psychic,
    Necrotic,
    Radiant,
    Force
}

public enum ActionType
{
    Attack,
    BonusAction,
    Reaction,
    MythicAction,
    DomainAction
}

public enum SizeCategory
{
    Tiny,
    Small,
    MediumSmall,
    Medium,
    Large,
    Huge,
    Gargantuan
}

public enum Alignment
{
    LawfulGood,
    NeutralGood,
    ChaoticGood,
    LawfulNeutral,
    TrueNeutral,
    ChaoticNeutral,
    LawfulEvil,
    NeutralEvil,
    ChaoticEvil
}

public enum SkillType
{
    Acrobatics,
    AnimalHandling,
    Arcana,
    Athletics,
    Deception,
    History,
    Insight,
    Intimidation,
    Investigation,
    Medicine,
    Nature,
    Perception,
    Performance,
    Persuasion,
    Religion,
    SleightOfHand,
    Stealth,
    Survival
}

public enum LanguageType
{
    Common,
    Dwarvish,
    Elvish,
    Giant,
    Gnomish,
    Goblin,
    Halfling,
    Orc,
    Abyssal,
    Celestial,
    Draconic,
    DeepSpeech,
    Infernal,
    Primordial,
    Sylvan,
    Undercommon
}

public enum CreatureType
{
    Humanoid,
    Fey,
    Fiend,
    Celestial,
    Undead,
    Construct,
    Dragon,
    Elemental
}

public enum SchoolOfMagicEnum
{
    Abjuration,
    Conjuration,
    Divination,
    Enchantment,
    Evocation,
    Illusion,
    Necromancy,
    Transmutation
}

public enum ItemCategory
{
    Weapon,
    Armor,
    Consumable,
    Tool,
    AdventuringGear,
    Trinket
}

public enum SpellComponent
{
    V,
    S,
    M
}

public enum SpellRange
{
    Self,
    Touch,
    Ranged
}

public enum SpellDuration
{
    Instantaneous,
    Concentration,
    UpTo1Minute,
    UpTo10Minutes,
    UpTo1Hour,
    UpTo2Hours,
    UpTo8Hours,
    UpTo24Hours,
    UpTo1Day,
    UntilDispelled,
    UntilDispelledOrTriggered,
    _1Minute,
    _10Minutes,
    _1Hour,
    _8Hours,
    _24Hours,
    _1Day,
    _7Days,
    _10Days,
    _30Days,
    _1Round,
    Special,
    Permanent
}

public enum SpellLevel
{
    Cantrip = 0,
    Level1 = 1,
    Level2 = 2,
    Level3 = 3,
    Level4 = 4,
    Level5 = 5,
    Level6 = 6,
    Level7 = 7,
    Level8 = 8,
    Level9 = 9,
    All = 10
}

public enum CastingTime
{
    Action,
    BonusAction,
    Reaction,
    Minute,
    Hour,
    Special
}

public enum RestType
{
    ShortRest,
    LongRest
}

public enum SpellConcentration
{
    No,
    Yes
}

public enum FeatPrerequisiteType
{
    AttributeMinimum,
    Proficiency,
    Spellcasting,
    RequiredLevel,
    FeatureRequired,
    GrantedProfiency
}

public enum ArmorProficiency
{
    Light,
    Medium,
    Heavy,
    Shield,
    None
}

public enum WeaponProficiency
{
    Simple,
    Martial,
    MartialLight,
    MartialFinesse,
    Exotic
}

public enum CategoryFeat
{
    General,
    Origin,
    CombatStyle,
    EpicFeat
}

public enum RangeDistanceType
{
    Feet,
    Meters
}

public enum ToolProficiency
{
    ThievesTools,
    DisguiseKit,
    ForgeryKit,
    HerbalismKit,
    NavigatorTools,
    PoisonersKit,
    SmithsTools,
    TinkersTools,
    VehiclesLand,
    VehiclesWater,
    ThreeMusicalInstruments,
    ArtisanToolsOrMusicalInstruments,
}

public enum SkillProficiency
{
    Acrobatics,
    AnimalHandling,
    Arcana,
    Athletics,
    Deception,
    History,
    Insight,
    Intimidation,
    Investigation,
    Medicine,
    Nature,
    Perception,
    Performance,
    Persuasion,
    Religion,
    SleightOfHand,
    Stealth,
    Survival
}

public enum ResourceType
{
    RagesCount,
    RageDamage,
    WeaponFocusCount,

    InspirationDie,
    CantripsKnown,
    PreparedSpellsCount,

    SecretsKnown,
    BondedSlots,
    SlotLevel,

    DivineChannelUses,
    SpellSlots,

    BeastFormUses,

    ChosenFoeUses,

    ResurgenceUses,

    SpellPoints,

    UnarmedDie,
    FocusPoints,
    UnrestrictedMovement,

    DivineChannelCount,

    PrecisionDie
}

public enum CodexKey
{
    None,
    Classes,
    Races,
    Backgrounds,
    Spells,
    Feats,
    Items,
    Rules
}

public enum LocEntity
{
    Race,
    SubRace,
    Class,
    Subclass,
    Background,
    Feat,
    Spell,
    Item,
    Monster,
    Condition,
    Action,
    Trait,
    SpecialTrait,
    Language,
    Proficiency,
    Feature,
    Character,
    SchoolOfMagic,
    ItemTemplate,
    Skill
}

public enum LocProperty
{
    Name,
    Description,
    AdditionalInfo,
    Resistances,
    Lore,
    ShortDescription,
    LongDescription,
    EffectDescription,
    UsageDescription,
    MaterialComponentDescription,
    ToolProficiencies,
    Equipment,
    Ability
}

public enum LocLanguage
{
    en,
    es,
    fr,
    de,
    it,
    pt,
    ru,
    ja,
    zh,
    ko,
    ar,
    hi,
    tr,
    pl,
    nl,
    sv,
    no,
    da,
    fi,
    cs,
    ro,
    hu,
    el,
}

public enum CharacterTab
{
    Spells,
    Skills,
    Features
}