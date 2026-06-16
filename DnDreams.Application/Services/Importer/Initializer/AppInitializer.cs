using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.Services.Importer.Initializer
{
    public class AppInitializer : IAppInitializer
    {
        // Propiedad que expone si la base de datos ya está lista
        public bool IsDatabaseReady { get; private set; } = false;
        public event Action? OnInitializationCompleted;

        public string CurrentStatusMessage { get; private set; } = "Lanzando iniciativa...";
        public event Action<string>? OnStatusMessageChanged;

        public void UpdateStatus(string newMessage)
        {
            CurrentStatusMessage = newMessage;
            OnStatusMessageChanged?.Invoke(newMessage);
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
                // Pase lo que pase, liberamos el Splash
                IsDatabaseReady = true;
                OnInitializationCompleted?.Invoke();
            }
        }
    }
}
