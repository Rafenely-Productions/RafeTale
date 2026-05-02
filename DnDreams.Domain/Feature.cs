namespace DnDreams.Domain; 

public class Feature
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Indica si el jugador debe elegir algo (ej: un hechizo o subclase)
    public bool RequiresChoice { get; set; }
}