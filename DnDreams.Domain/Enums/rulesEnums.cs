namespace DnDreams.Domain.Enums;

// Las 15 condiciones oficiales de D&D 5e
[Flags] // Nos permite combinar estados en una sola columna binaria (ej: Envenenado Y Derribado)
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

// Propiedades exactas del personaje que pueden verse alteradas por buffs/debuffs
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

// Duración estricta basada en las reglas para saber cuándo limpiar un modificador
public enum ModifierDurationType
{
    Instantaneous,   // Daño, curación inmediata
    UntilEndOfTurn,  // Dura el turno actual
    Rounds,          // Tiene un contador de asaltos (combate)
    UntilShortRest,  // Se limpia en descanso corto
    UntilLongRest,   // Se limpia al dormir (la mayoría)
    Permanent        // Objetos mágicos o dotes (mientras estén activos/equipados)
}
public enum ModifierType
{
    AttributeBonus, // Ej: +1 a Carisma, +2 a Fuerza
    GrantSpell,     // Ej: Te da el hechizo "Escudo"
    GrantFeature,   // Ej: Te da la habilidad "Visión en la Oscuridad"
    HpBonus,        // Ej: El dote Robustez da +2 HP por nivel
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
    ExpandBrutalStrikeEffects,
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
    AlwaysPreparedSpells,
    GrantInvocations,
    EnablePactMagic,
    ShortRitualTrigger,
    ScalePactSlots,
    GrantMysticArcanum,
    EnhanceFeature,
    SelectChoice,
    SelectFeat,
    ModifyFeature,
    GrantLanguage,
    SelectOption,
    UpgradeFeature,
    AlwaysPreparedSpell,
    GrantLanguages,
    ModifyAttacksPerAction,
    GrantSense,
    UnlockWeaponMasterySlots,
    GrantAlwaysPreparedSpell,
    GrantFeatCategory,
    ModifyAction,
    EmanationAura,
    Action,
    ModifyAura,
    OnHitModifier,
    BonusActionOptions,
    BonusAction,
    Reaction,
    UnlockSneakAttackOptions,
    PassiveEffect,
    GrantSaveProficiency,
    FeatureCooldown,

}

public enum ASI
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
    LegendaryAction,
    LairAction
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
    V, //Verbal
    S,//Somatic
    M//Material
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
    Level9 = 9
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
    Yes,
    No
}
public enum FeatPrerequisiteType
{
    AttributeMinimum, // Ej: Fuerza 13
    Proficiency,      // Ej: Competencia con armas marciales
    Spellcasting,     // Ej: Capacidad de lanzar hechizos de nivel 1 o superior
    RequiredLevel,
    FeatureRequired,
    GrantedProfiency
}
public enum ArmorProficiency
{
    Light,
    Medium,
    Heavy,
    Shield
}
public enum WeaponProficiency
{
    Simple,
    Martial,
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
    VehiclesWater
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
    WeaponMasteryCount,

    BardicDie,
    CantripsKnown,
    PreparedSpellsCount,

    InvocationsKnown,
    PactSlots,
    SlotLevel,

    ChannelDivinityUses,
    SpellSlots,

    WildShapeCount,

    FavoredEnemyUses,

    SecondWindUses,

    SorceryPoints,

    MartialArtsDie,
    FocusPoints,
    UnarmoredMovement,

    ChannelDivinityCount,

    SneakAttackDie
}
