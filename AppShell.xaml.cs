using SAAD2;
using SAAD2.Enums;
using SAAD2.Helpers;
using SAAD2.Models;
using SAAD2.Services;
using SAAD2.Views;

namespace SAAD2
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
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

            // Se o alvo da navegação for nulo, não faz nada.
            if (args.Target == null)
            {
                return;
            }

            var isLoggedIn = Preferences.Get("IsLoggedIn", false);
            var targetRoute = args.Target.Location.OriginalString;

            // Lista de rotas que NÃO precisam de login
            var publicRoutes = new[] { "MainPage", "RecuperarSenhaPage", "RegistroPage" };

            // Se o usuário NÃO está logado E a rota de destino NÃO é uma rota pública
            if (!isLoggedIn && !publicRoutes.Any(p => targetRoute.Contains(p)))
            {
                // Cancela a navegação atual
                args.Cancel();

                // Força a navegação para a página de login de uma maneira segura,
                // garantindo que não aconteça no meio de outra operação.
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await GoToAsync("//MainPage");
                });
            }
        }
        // ADICIONE ESTE MÉTODO COMPLETO
        private void OnThemeToggleButtonClicked(object sender, EventArgs e)
        {
            // Pega a instância atual do App
            var app = (App)Application.Current;

            // Verifica o tema atual e define o novo tema
            var novoTema = app.CurrentTheme == Theme.Light ? Theme.Dark : Theme.Light;

            // Chama o método SetTheme que criamos no App.xaml.cs
            app.SetTheme(novoTema);
        }
    }
}