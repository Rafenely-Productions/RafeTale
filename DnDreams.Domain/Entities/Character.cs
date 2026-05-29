using DnDreams.Domain.Enums;
using DnDreams.Domain.Interfaces;
namespace DnDreams.Domain.Entities;

public class Character : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Atributos base
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

    public List<Feature> AcquiredFeatures { get; set; } = new();
    public List<ClassLevelProgression> ClassLevels { get; set; } = new();

    public Guid RaceId { get; set; }
    public Race Race { get; set; } = null!;

    public Guid ClassDefId { get; set; }
    public ClassDefinition ClassDef { get; set; } = null!;

    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public Dictionary<string, int> Stats { get; set; } = new();
    public virtual ICollection<Feat> AcquiredFeats { get; set; } = new List<Feat>();
    public virtual ICollection<Spell> KnownSpells { get; set; } = new List<Spell>();
    public virtual ICollection<CharacterModifier> CharacterModifiers { get; set; } = new List<CharacterModifier>();

    public List<CharacterInventory> Inventory { get; set; } = new();

    public CharacterStatus Status { get; set; } = null!;
    public List<CharacterSpellSlots> SpellSlots { get; set; } = new();
    public List<ActiveModifiers> ActiveModifiers { get; set; } = new();
    public List<CampaignCharacter> CampaignCharacters { get; set; } = new();
    public int ProficiencyBonus => 2 + ((Level - 1) / 4); // Lógica oficial de D&D

    public int GetSkillBonus(string skillName, string baseStat)
    {
        // 1. Obtenemos el modificador del atributo base (Fuerza, Destreza, etc.)
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

        // 2. Buscamos si el personaje tiene competencia (Proficiency) en esta skill
        // Por ahora, asumiremos que tienes una lista de strings con las skills entrenadas
        bool isProficient = AcquiredFeatures.Any(f => f.Name.Contains(skillName));

        return statMod + (isProficient ? ProficiencyBonus : 0);
    }
}