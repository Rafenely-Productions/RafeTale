namespace DnDreams.Application.Services.Importer.Initializer;

public class AppInitializer : IAppInitializer
{
    public bool IsDatabaseReady { get; private set; } = false;
    public string CurrentStatusMessage { get; private set; } = "Lanzando iniciativa...";

    public void UpdateStatus(string newMessage)
    {
        CurrentStatusMessage = newMessage;
    }

    public async Task InitializeAsync(Func<Task> coreDataLoadingTask)
    {
        try
        {
            await coreDataLoadingTask();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en inicialización global: {ex.Message}");
        }
        finally
        {
            IsDatabaseReady = true;
        }
    }
}