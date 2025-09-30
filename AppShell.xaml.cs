using SAAD2.Enums;
using SAAD2.Helpers;
using SAAD2.Views;

namespace SAAD2
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
            UpdateThemeIcon(); // Define o ícone correto na inicialização
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute("RecuperarSenhaPage", typeof(RecuperarSenhaPage));
            Routing.RegisterRoute("RegistroPage", typeof(RegistroPage));
            Routing.RegisterRoute("LogoutPage", typeof(LogoutPage));
            Routing.RegisterRoute("MateriasPage", typeof(MateriasPage));
            Routing.RegisterRoute("RegistroMateriasPage", typeof(RegistroMateriasPage));
            Routing.RegisterRoute("FaltasPage", typeof(FaltasPage));
            Routing.RegisterRoute("RegistroFaltasPage", typeof(RegistroFaltasPage));
            Routing.RegisterRoute(nameof(FaceAuthPage), typeof(FaceAuthPage));
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);
            if (args.Target == null) return;
            var isLoggedIn = Preferences.Get("IsLoggedIn", false);
            var targetRoute = args.Target.Location.OriginalString;
            var publicRoutes = new[] { "MainPage", "RecuperarSenhaPage", "RegistroPage" };

            if (!isLoggedIn && !publicRoutes.Any(p => targetRoute.Contains(p)))
            {
                args.Cancel();
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await GoToAsync("//MainPage");
                });
            }
        }

        private void OnThemeToggleButtonClicked(object sender, EventArgs e)
        {
            var app = (App)Application.Current;
            var novoTema = app.CurrentTheme == Theme.Light ? Theme.Dark : Theme.Light;
            app.SetTheme(novoTema);
            UpdateThemeIcon();
        }

        private void UpdateThemeIcon()
        {
            var app = (App)Application.Current;
            ThemeButton.Text = app.CurrentTheme == Theme.Light
                ? MaterialIconFont.WhiteBalanceSunny
                : MaterialIconFont.WeatherNight;
        }
    }
}