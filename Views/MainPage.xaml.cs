using SAAD2.Views;
using Firebase.Auth;
using Firebase.Auth.Providers;
using System.Threading;
using SAAD2.Enums;   // Adicionado
using SAAD2.Helpers; // Adicionado

namespace SAAD2.Views
{
    public partial class MainPage : ContentPage
    {
        // Conteúdo original mesclado com as novas adições
        private const string FirebaseApiKey = "AIzaSyCW4PQCcScohZJTo4IfevkCRxxXbmQY7HA";
        private readonly FirebaseAuthClient client;

        public MainPage()
        {
            InitializeComponent();
            client = new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = FirebaseApiKey,
                AuthDomain = "saad-1fd38.firebaseapp.com",
                Providers = new FirebaseAuthProvider[] { new EmailProvider() }
            });
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            UpdateThemeIcon(); // Garante que o ícone esteja correto quando a página aparece

            var biometricEnabled = Preferences.Get("BiometricEnabled", false);
            if (biometricEnabled)
            {
                await Task.Delay(250);
            }
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                await DisplayAlert("Erro", "Usuário e senha são obrigatórios.", "OK");
                return;
            }
            try
            {
                var userCredential = await client.SignInWithEmailAndPasswordAsync(UsernameEntry.Text, PasswordEntry.Text);
                if (userCredential != null && !string.IsNullOrEmpty(userCredential.User.Uid))
                {
                    // Salva o ID do usuário para o acesso ao Firebase
                    Preferences.Set("UserUid", userCredential.User.Uid);

                    await PromptToEnableBiometricsAsync();
                    Preferences.Set("IsLoggedIn", true);
                    await Shell.Current.GoToAsync("//HomePage");
                }
            }
            catch (FirebaseAuthException)
            {
                await DisplayAlert("Erro de Login", "Credenciais inválidas.", "OK");
            }
        }

        private async Task PromptToEnableBiometricsAsync()
        {
            bool enableBiometrics = await DisplayAlert("Login Rápido", "Deseja habilitar o login com biometria (Facial/Digital) para o próximo acesso?", "Sim", "Não");
            if (enableBiometrics)
            {
                Preferences.Set("BiometricEnabled", true);
                await SecureStorage.Default.SetAsync("auth_email", UsernameEntry.Text);
                await SecureStorage.Default.SetAsync("auth_password", PasswordEntry.Text);
            }
            else
            {
                Preferences.Set("BiometricEnabled", false);
                SecureStorage.Default.Remove("auth_email");
                SecureStorage.Default.Remove("auth_password");
            }
        }

        // Métodos do botão de tema
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

        private async void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("RecuperarSenhaPage");
        }

        private async void OnRegisterTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("RegistroPage");
        }
    }
}