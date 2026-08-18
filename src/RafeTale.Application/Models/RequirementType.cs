namespace RafeTale.Application.Models
{
    public enum RequirementType
    {
        Choice,       // El usuario debe elegir (Subclase, Hechizo)
        StatIncrease, // Aumento de atributos (nivel 4, 8, etc)
        Informational // Solo avisar (ej: "Ahora tienes Ataque Adicional")
    }
}