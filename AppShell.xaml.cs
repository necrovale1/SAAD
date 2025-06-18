using SAAD.Resources.Styles;

namespace SAAD;

public partial class AppShell : Shell
{
    public AppShell()
    {

        InitializeComponent();

        // Use Dispatcher instead of obsolete Device.InvokeOnMainThreadAsync
        Dispatcher.DispatchAsync(async () =>
        {
            await Task.Delay(2000); // Wait for 2 seconds

            // Navigate to main page
            await Current.GoToAsync("//main");

        });

        // Register navigation routes (moved outside the async block)
        Routing.RegisterRoute("HomePage", typeof(HomePage));
        Routing.RegisterRoute("MainPage", typeof(MainPage));
        Routing.RegisterRoute("Registro", typeof(Registro));
        Routing.RegisterRoute("RecuperarSenha", typeof(RecuperarSenha));
        Routing.RegisterRoute("FaltasPage", typeof(FaltasPage));
        Routing.RegisterRoute("MateriasPage", typeof(MateriasPage));
    }

}