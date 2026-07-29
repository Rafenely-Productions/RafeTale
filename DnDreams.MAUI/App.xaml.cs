namespace DnDreams.MAUI;

public partial class App : Microsoft.Maui.Controls.Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new MainPage()) { Title = "DnDreams.MAUI" };

        // 🌟 Define el tamaño inicial de la ventana en modo escritorio (simulando móvil)
        window.Width = 410;
        window.Height = 850;

        // (Opcional) Puedes fijar límites para que no la hagan demasiado pequeña
        window.MinimumWidth = 380;
        window.MinimumHeight = 650;

        return window;
    }
}