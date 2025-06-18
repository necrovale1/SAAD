using SAAD.Resources.Styles;

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
        await Task.Delay(3000);

        // Safely check if Application.Current is not null before accessing MainPage
        if (Application.Current != null)
        {
            Application.Current.MainPage = new MainPage();
        }
    }
}
