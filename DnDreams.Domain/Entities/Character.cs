namespace DnDreams.Domain.Entities;

public class Character
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Atributos base
    public int Strength { get; set; } = 10;
    public int Dexterity { get; set; } = 10;
    public int Constitution { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public int Wisdom { get; set; } = 10;
    public int Charisma { get; set; } = 10;

    public int StrModifier => CalculateModifier(Strength);
    public int DexModifier => CalculateModifier(Dexterity);
    public int ConModifier => CalculateModifier(Constitution);
    public int IntModifier => CalculateModifier(Intelligence);
    public int WisModifier => CalculateModifier(Wisdom);
    public int ChaModifier => CalculateModifier(Charisma);

    private int CalculateModifier(int score) => (int)Math.Floor((score - 10) / 2.0);

    public List<Feature> AcquiredFeatures { get; set; } = new();
    public List<ClassLevelProgression> ClassLevels { get; set; } = new();
}