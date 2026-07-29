namespace DnDreams.Application.Services.Importer.Initializer;

public class AppInitializer : IAppInitializer
{
    private readonly TaskCompletionSource _tcs = new();

    public bool IsDatabaseReady { get; private set; } = false;
    public string CurrentStatusMessage { get; private set; } = "Lanzando iniciativa...";

    // ⬇️ NUEVO: Task que los componentes pueden await
    public Task InitializationTask => _tcs.Task;

    public event Action? OnInitializationCompleted;
    public event Action? OnStatusMessageChanged;

    public void UpdateStatus(string newMessage)
    {
        CurrentStatusMessage = newMessage;
        OnStatusMessageChanged?.Invoke();
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
            _tcs.SetResult();           // ⬅️ Libera el Task
            OnInitializationCompleted?.Invoke();
        }
    }
}