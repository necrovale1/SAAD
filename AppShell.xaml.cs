using Microsoft.Maui.Controls;

namespace SAAD;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
        InitializeNavigation();
    }

    private void RegisterRoutes()
    {
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(Registro), typeof(Registro));
        Routing.RegisterRoute(nameof(RecuperarSenha), typeof(RecuperarSenha));
        Routing.RegisterRoute(nameof(MateriasPage), typeof(MateriasPage));
        Routing.RegisterRoute(nameof(FaltasPage), typeof(FaltasPage));
        Routing.RegisterRoute(nameof(LogoutPage), typeof(LogoutPage));
    }

    private void InitializeNavigation()
    {
        if (Preferences.Get("UsuarioLogado", false))
        {
                        // New way
            Dispatcher.Dispatch(() =>
            {
                Task.Delay(1000); // Small delay for better UX
                CurrentItem = FindByName("HomePage") as ShellItem;
            });
        }
    }
}