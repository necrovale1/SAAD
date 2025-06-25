
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

        // Define a página principal como o AppShell, que controla a navegação.
        if (Application.Current != null)
        {
            Application.Current.MainPage = new AppShell();
        }
    }
}