namespace DnDreams.Application.Services.Importer.Initializer
{
    public interface IAppInitializer
    {
        public bool IsDatabaseReady { get;  }
        public event Action? OnInitializationCompleted;

        public string CurrentStatusMessage { get; }
        public event Action<string>? OnStatusMessageChanged;

        public void UpdateStatus(string newMessage);

        public Task InitializeAsync(Func<Task> coreDataLoadingTask);
    }
}