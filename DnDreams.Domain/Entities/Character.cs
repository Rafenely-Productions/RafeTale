using DnDreams.Domain.Enums;
using DnDreams.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDreams.Domain.Entities;

public class Character : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string History { get; set; } = string.Empty; // 🔮 Añadido para el Lore/Trasfondo escrito

    // --- Atributos Dinámicos ---
    public int Strength
    {
        get
        {
            int baseVal = Stats.TryGetValue(TargetPropertyType.Strength.ToString(), out var val) ? val : 10;
            int bonus = CharacterModifiers.Where(m => m.Type == ModifierType.AttributeBonus && m.Target == TargetPropertyType.Strength.ToString()).Sum(m => m.Value);
            return baseVal + bonus;
        }
        set => Stats[TargetPropertyType.Strength.ToString()] = value;
    }
    public int Dexterity
    {
        get
        {
            int baseVal = Stats.TryGetValue(TargetPropertyType.Dexterity.ToString(), out var val) ? val : 10;
            int bonus = CharacterModifiers.Where(m => m.Type == ModifierType.AttributeBonus && m.Target == TargetPropertyType.Dexterity.ToString()).Sum(m => m.Value);
            return baseVal + bonus;
        }
        set => Stats[TargetPropertyType.Dexterity.ToString()] = value;
    }
    public int Constitution
    {
        get
        {
            int baseVal = Stats.TryGetValue(TargetPropertyType.Constitution.ToString(), out var val) ? val : 10;
            int bonus = CharacterModifiers.Where(m => m.Type == ModifierType.AttributeBonus && m.Target == TargetPropertyType.Constitution.ToString()).Sum(m => m.Value);
            return baseVal + bonus;
        }
        set => Stats[TargetPropertyType.Constitution.ToString()] = value;
    }
    public int Intelligence
    {
        get
        {
            int baseVal = Stats.TryGetValue(TargetPropertyType.Intelligence.ToString(), out var val) ? val : 10;
            int bonus = CharacterModifiers.Where(m => m.Type == ModifierType.AttributeBonus && m.Target == TargetPropertyType.Intelligence.ToString()).Sum(m => m.Value);
            return baseVal + bonus;
        }
        set => Stats[TargetPropertyType.Intelligence.ToString()] = value;
    }
    public int Wisdom
    {
        get
        {
            int baseVal = Stats.TryGetValue(TargetPropertyType.Wisdom.ToString(), out var val) ? val : 10;
            int bonus = CharacterModifiers.Where(m => m.Type == ModifierType.AttributeBonus && m.Target == TargetPropertyType.Wisdom.ToString()).Sum(m => m.Value);
            return baseVal + bonus;
        }
        set => Stats[TargetPropertyType.Wisdom.ToString()] = value;
    }
    public int Charisma
    {
        get
        {
            int baseVal = Stats.TryGetValue(TargetPropertyType.Charisma.ToString(), out var val) ? val : 10;
            int bonus = CharacterModifiers.Where(m => m.Type == ModifierType.AttributeBonus && m.Target == TargetPropertyType.Charisma.ToString()).Sum(m => m.Value);
            return baseVal + bonus;
        }
        set => Stats[TargetPropertyType.Charisma.ToString()] = value;
    }

    // --- Modificadores de Atributo ---
    public int StrModifier => CalculateModifier(Strength);
    public int DexModifier => CalculateModifier(Dexterity);
    public int ConModifier => CalculateModifier(Constitution);
    public int IntModifier => CalculateModifier(Intelligence);
    public int WisModifier => CalculateModifier(Wisdom);
    public int ChaModifier => CalculateModifier(Charisma);

    private int CalculateModifier(int score)
    {
        return (int)Math.Floor((score - 10) / 2.0);
    }

    // --- Estadísticas Vitales (Mapeadas para SQLite) ---
    public int MaxHp { get; set; } = 10;
    public int CurrentHp { get; set; } = 10;
    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public Dictionary<string, int> Stats { get; set; } = new();

    // --- Relaciones de Origen y Reglas 2024 ---
    public Guid RaceId { get; set; }
    public Race Race { get; set; } = null!;

    public Guid ClassDefId { get; set; } // Ajustado para coincidir con tu propiedad 'ClassDef'
    public ClassDefinition ClassDef { get; set; } = null!;

    public Guid BackgroundId { get; set; } // 🔮 Añadido para guardar de qué trasfondo viene el héroe
    public Background Background { get; set; } = null!;

    // --- Colecciones y Grafos Relacionales ---
    public List<Feature> AcquiredFeatures { get; set; } = new();
    public List<ClassLevelProgression> ClassLevels { get; set; } = new();
    public virtual ICollection<Feat> AcquiredFeats { get; set; } = new List<Feat>();
    public virtual ICollection<Spell> KnownSpells { get; set; } = new List<Spell>();
    public virtual ICollection<CharacterModifier> CharacterModifiers { get; set; } = new List<CharacterModifier>();
    public List<CharacterInventory> Inventory { get; set; } = new();
    public CharacterStatus Status { get; set; } = null!;
    public List<CharacterSpellSlots> SpellSlots { get; set; } = new();
    public List<ActiveModifiers> ActiveModifiers { get; set; } = new();
    public List<CampaignCharacter> CampaignCharacters { get; set; } = new();

    // --- Mecánicas de Juego ---
    public int ProficiencyBonus => 2 + ((Level - 1) / 4);

    public int GetSkillBonus(string skillName, string baseStat)
    {
        int statMod = baseStat switch
        {
            "Strength" => StrModifier,
            "Dexterity" => DexModifier,
            "Constitution" => ConModifier,
            "Intelligence" => IntModifier,
            "Wisdom" => WisModifier,
            "Charisma" => ChaModifier,
            _ => 0
        };

        bool isProficient = AcquiredFeatures.Any(f => f.TechnicalName.Contains(skillName));
        return statMod + (isProficient ? ProficiencyBonus : 0);
    }
}