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

    protected override async void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

        // Rotas que exigem que o usuário esteja logado.
        var protectedRoutes = new[] { nameof(HomePage), nameof(MateriasPage), nameof(FaltasPage), nameof(RegistroMateriasPage), nameof(RegistroFaltasPage) };

        // Extrai o nome da rota base, removendo o prefixo "//" e parâmetros de query.
        string targetRoute = args.Target.Location.OriginalString;
        if (targetRoute.StartsWith("//"))
        {
            targetRoute = targetRoute.Substring(2);
        }
        var routeName = targetRoute.Contains("?") ? targetRoute.Split('?')[0] : targetRoute;

        // Verifica se o usuário está logado.
        bool isUserLoggedIn = Preferences.Get("UsuarioLogado", false);

        // Se o usuário não estiver logado e tentar acessar uma rota protegida...
        if (!isUserLoggedIn && protectedRoutes.Contains(routeName))
        {
            args.Cancel(); // Cancela a navegação atual.

            // Redireciona para a página de login, garantindo que não haja referência nula.
            if (Current != null)
            {
                await Current.GoToAsync($"//{nameof(MainPage)}", false);
            }
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