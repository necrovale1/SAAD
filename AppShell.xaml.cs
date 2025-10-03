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
            UpdateThemeIcon();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(HomePage), typeof(Views.HomePage));
            Routing.RegisterRoute(nameof(MainPage), typeof(Views.MainPage));
            Routing.RegisterRoute(nameof(Views.LoginPage), typeof(Views.LoginPage));
            Routing.RegisterRoute(nameof(Views.RegistroPage), typeof(Views.RegistroPage));
            Routing.RegisterRoute(nameof(Views.RecuperarSenhaPage), typeof(Views.RecuperarSenhaPage));
            Routing.RegisterRoute(nameof(Views.FaltasPage), typeof(Views.FaltasPage));
            Routing.RegisterRoute(nameof(Views.RegistroFaltasPage), typeof(Views.RegistroFaltasPage));
            Routing.RegisterRoute(nameof(Views.MateriasPage), typeof(Views.MateriasPage));
            Routing.RegisterRoute(nameof(Views.RegistroMateriasPage), typeof(Views.RegistroMateriasPage));
            Routing.RegisterRoute(nameof(Views.FaceAuthPage), typeof(Views.FaceAuthPage));
            Routing.RegisterRoute(nameof(Views.LogoutPage), typeof(Views.LogoutPage));
            Routing.RegisterRoute(nameof(Views.UserConfigPage), typeof(Views.UserConfigPage));

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

        // CORREÇÃO APLICADA AQUI: EventArgs foi trocado por TappedEventArgs
        private void OnThemeIconTapped(object sender, TappedEventArgs e)
        {
            var app = (App)Application.Current;
            ThemeIconLabel.Text = app.CurrentTheme == Theme.Light
            ? FontAwesomeIcons.Sun
            : FontAwesomeIcons.Moon;
        }

        private void UpdateThemeIcon()
        {
            var app = (App)Application.Current;
            ThemeIconLabel.Text = app.CurrentTheme == Theme.Light
                ? MaterialIconFont.WhiteBalanceSunny
                : MaterialIconFont.WeatherNight;
        }
    }
}