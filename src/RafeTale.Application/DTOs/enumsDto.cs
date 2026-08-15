
namespace RafeTale.Application.DTOs
{
    public enum ModifierTypeDto
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
        UnlockWeaponExpertise,        // Antes: UnlockWeaponMastery
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
        GrantArcaneInvocations,       // Antes: GrantInvocations
        EnableBondedMagic,            // Antes: EnablePactMagic
        ShortRitualTrigger,
        ScaleBondedSlots,             // Antes: ScalePactSlots
        GrantMysticSecrets,           // Antes: GrantMysticArcanum
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
        UnlockWeaponExpertiseSlots,   // Antes: UnlockWeaponMasterySlots
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
        UnlockPrecisionStrikeOptions, // Antes: UnlockSneakAttackOptions
        PassiveEffect,
        GrantSaveProficiency,
        FeatureCooldown,

    }
}
