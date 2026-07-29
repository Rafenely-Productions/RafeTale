namespace DnDreams.Application.Services.Importer.Initializer;

public interface IAppInitializer
{
    bool IsDatabaseReady { get; }
    string CurrentStatusMessage { get; }
    Task InitializationTask { get; }

    Task InitializeAsync(Func<Task> coreDataLoadingTask);
    void UpdateStatus(string newMessage);
}