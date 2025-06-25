namespace SAAD;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
        ApplyCurrentTheme();
    }

    private void RegisterRoutes()
    {
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(Registro), typeof(Registro));
        Routing.RegisterRoute(nameof(RecuperarSenha), typeof(RecuperarSenha));
        Routing.RegisterRoute(nameof(MateriasPage), typeof(MateriasPage));
        Routing.RegisterRoute(nameof(RegistroFaltasPage), typeof(RegistroFaltasPage));
        Routing.RegisterRoute(nameof(FaltasPage), typeof(FaltasPage));
        Routing.RegisterRoute(nameof(LogoutPage), typeof(LogoutPage));
    }

    protected override void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

        var protectedRoutes = new[] { nameof(HomePage), nameof(MateriasPage), nameof(FaltasPage) };
        var targetRoute = args.Target.Location.OriginalString;
        var routeName = targetRoute.Contains("?") ? targetRoute.Split('?')[0] : targetRoute;
        routeName = routeName.Replace("//", "");

        bool isUserLoggedIn = Preferences.Get("UsuarioLogado", false);

        if (!isUserLoggedIn && protectedRoutes.Contains(routeName))
        {
            args.Cancel();

            // Esta é a mudança crucial. Agendamos a navegação para ocorrer
            // logo após o término do evento atual, evitando o deadlock.
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Current != null)
                {
                    await Current.GoToAsync($"//{nameof(MainPage)}", false);
                }
            });
        }
    }

    private void ApplyCurrentTheme()
    {
        var isDark = Preferences.Get("DarkTheme", false);
        // 3. Correção: Adicionada verificação de nulo para Application.Current.
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
        }
    }

    private void OnThemeToggleClicked(object sender, EventArgs e)
    {
        var isDark = !Preferences.Get("DarkTheme", false);
        Preferences.Set("DarkTheme", isDark);
        // 3. Correção: Adicionada verificação de nulo também aqui.
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
        }
    }
}