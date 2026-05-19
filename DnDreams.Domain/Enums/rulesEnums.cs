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
    AttributeBonus,  // Ej: +1 a Carisma, +2 a Fuerza
    GrantSpell,      // Ej: Te da el hechizo "Escudo"
    GrantFeature,    // Ej: Te da la habilidad "Visión en la Oscuridad"
    HpBonus          // Ej: El dote Robustez da +2 HP por nivel
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