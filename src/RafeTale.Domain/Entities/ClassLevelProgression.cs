using RafeTale.Domain.Interfaces;
using RafeTale.Domain.Modifiers;

namespace RafeTale.Domain.Entities;

public class ClassLevelProgression : IEntity
{
    public Guid Id { get; set; }
    public int Level { get; set; }
    public List<Feature> Features { get; set; } = [];
    public List<ClassTrait> Traits { get; set; } = [];
    public ClassDefinition? ClassDef { get; set; }
    public Guid ClassDefId { get; set; }

}