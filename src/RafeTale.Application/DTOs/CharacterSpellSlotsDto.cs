namespace RafeTale.Application.DTOs
{
    public class CharacterSpellSlotsDto 
    {
        public Guid Id { get; set; } // Tu clave primaria física
        public Guid CharacterId { get; set; } // Tu FK
        public virtual CharacterDto Character { get; set; } = null!; // Propiedad de navegación

        public int Level { get; set; }
        public int MaxSlots { get; set; }
        public int UsedSlots { get; set; }

        // Propiedad calculada para Blazor/MAUI
        public int RemainingSlots => Math.Max(0, MaxSlots - UsedSlots);
    }
}
