namespace DnDreams.Domain.Entities;

public class Character
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Atributos base
    public int Strength
    {
        get
        {
            int baseVal = Stats.TryGetValue("Fuerza", out var val) ? val : 10;
            int bonus = ActiveModifiers.Where(m => m.Type == ModifierType.AttributeBonus && m.Target == "Strength").Sum(m => m.Value);
            return baseVal + bonus;
        }
        set => Stats["Fuerza"] = value;
    }
    public int Dexterity
    {
        get
        {
            int baseVal = Stats.TryGetValue("Destreza", out var val) ? val : 10;
            int bonus = ActiveModifiers.Where(m => m.Type == ModifierType.AttributeBonus && m.Target == "Dexterity").Sum(m => m.Value);
            return baseVal + bonus;
        }
        set => Stats["Destreza"] = value;
    }
    public int Constitution
    {
        get
        {
            int baseVal = Stats.TryGetValue("Constitución", out var val) ? val : 10;
            int bonus = ActiveModifiers.Where(m => m.Type == ModifierType.AttributeBonus && m.Target == "Constitution").Sum(m => m.Value);
            return baseVal + bonus;
        }
        set => Stats["Constitución"] = value;
    }
    public int Intelligence
    {
        get
        {
            int baseVal = Stats.TryGetValue("Inteligencia", out var val) ? val : 10;
            int bonus = ActiveModifiers.Where(m => m.Type == ModifierType.AttributeBonus && m.Target == "Intelligence").Sum(m => m.Value);
            return baseVal + bonus;
        }
        set => Stats["Inteligencia"] = value;
    }
    public int Wisdom
    {
        get
        {
            int baseVal = Stats.TryGetValue("Sabiduría", out var val) ? val : 10;
            int bonus = ActiveModifiers.Where(m => m.Type == ModifierType.AttributeBonus && m.Target == "Wisdom").Sum(m => m.Value);
            return baseVal + bonus;
        }
        set => Stats["Sabiduría"] = value;
    }
    public int Charisma
    {
        get
        {
            int baseVal = Stats.TryGetValue("Carisma", out var val) ? val : 10;
            int bonus = ActiveModifiers.Where(m => m.Type == ModifierType.AttributeBonus && m.Target == "Charisma").Sum(m => m.Value);
            return baseVal + bonus;
        }
        set => Stats["Carisma"] = value;
    }

    public int StrModifier => CalculateModifier(Strength);
    public int DexModifier => CalculateModifier(Dexterity);
    public int ConModifier => CalculateModifier(Constitution);
    public int IntModifier => CalculateModifier(Intelligence);
    public int WisModifier => CalculateModifier(Wisdom);
    public int ChaModifier => CalculateModifier(Charisma);

    private int CalculateModifier(int score)
    {
        if (score >20) score = 20;

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
    public virtual ICollection<CharacterModifier> ActiveModifiers { get; set; } = new List<CharacterModifier>();
}