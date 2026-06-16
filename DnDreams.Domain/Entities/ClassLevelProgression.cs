using DnDreams.Domain.Interfaces;
using DnDreams.Domain.Modifiers;

namespace DnDreams.Domain.Entities;

public class ClassLevelProgression : IEntity
{
    public Guid Id { get; set; }
    public int Level { get; set; }
    public List<Feature> Features { get; set; } = new();
    public List<ClassTrait> Traits = new List<ClassTrait>();
    public ClassDefinition? ClassDef { get; set; }
    public Guid ClassDefId { get; set; }

}