namespace SAAD;

public partial class SplashScreen : ContentPage
{
    public SplashScreen()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Aguarda 3 segundos para simular carregamento
        await Task.Delay(3000); //

        // Define a p�gina principal como o AppShell, que controla a navega��o.
        if (Application.Current != null)
        {
            Application.Current.MainPage = new AppShell();
        }
    }
}