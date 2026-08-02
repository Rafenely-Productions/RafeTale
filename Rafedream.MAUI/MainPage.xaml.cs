namespace Rafedream.MAUI;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    protected override bool OnBackButtonPressed()
    {
        // Si estás en la página de inicio (Root), puedes decidir si sales de la app o haces algo más
        // return true; // Si retornas true, "bloqueas" el botón atrás para que no haga nada.

        // Si quieres que se comporte de forma normal, déjalo con el comportamiento base:
        return base.OnBackButtonPressed();
    }
}
