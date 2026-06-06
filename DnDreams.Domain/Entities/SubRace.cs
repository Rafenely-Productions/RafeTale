using DnDreams.Domain.Interfaces;

namespace DnDreams.Domain.Entities
{
    public class SubRace : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TechnicalName { get; set; } = string.Empty;
        public Guid RaceId { get; set; }
        public Race Race { get; set; } = null!;
    }
}