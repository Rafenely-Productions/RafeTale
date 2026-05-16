using System;

namespace DnDreams.Domain.Entities;

public class CharacterStatus
{
    public Guid Id { get; set; }

    public Guid CharacterId { get; set; }
    public Character Character { get; set; } = null!;

    // Recursos temporales / vivos
    public int CurrentHp { get; set; }
    public int TemporaryHp { get; set; }

    // Rastreadores de tiros de salvación contra la muerte (por si cae a 0 HP)
    public int DeathSaveSuccesses { get; set; }
    public int DeathSaveFailures { get; set; }

    // Estados alterados (Podríamos manejar un enum flag o strings para simplificar)
    // Ej: "Envenenado", "Derribado", "Inconsciente"
    public string ActiveConditions { get; set; } = string.Empty;
}