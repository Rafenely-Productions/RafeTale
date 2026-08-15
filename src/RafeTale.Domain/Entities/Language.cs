using RafeTale.Domain.Interfaces;

namespace RafeTale.Domain.Entities
{
    public class Language : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TechnicalName { get; set; } = string.Empty;
        public IEnumerable<Race> Races { get; set; } = new List<Race>();
    }
}
