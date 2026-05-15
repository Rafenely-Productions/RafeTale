namespace DnDreams.Domain.Entities;

public class ClassLevelProgression
{
    public Guid Id { get; set; }
    public int Level { get; set; }
    public List<Feature> Features { get; set; } = new(); // Lo que ganas en este nivel
    public ClassDefinition? ClassDef { get; set; }
    public Guid ClassDefId { get; set; }

}