using DnDreams.Domain.Interfaces;


namespace DnDreams.Domain.Entities;

public class Subclass : IEntity
{
    public Guid Id { get; set; }
    public string TechnicalName { get; set; } = string.Empty;
    public int LevelRequirement { get; set; } = 3; 
    public Guid ClassDefinitionId { get; set; }
    public ClassDefinition ClassDefinition { get; set; } = null!;
    public virtual ICollection<SubclassLevelProgression> Progressions { get; set; } = new List<SubclassLevelProgression>();

}
