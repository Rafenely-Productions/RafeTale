namespace DnDreams.Domain.Entities
{
    public class SubRace
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RaceId { get; set; }
        public Race Race { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = "";
    }
}