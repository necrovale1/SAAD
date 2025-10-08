using Firebase.Auth;
using Firebase.Auth.Providers;
using SAAD2.Enums;
using SAAD2.Helpers;
using UserModel = SAAD2.Models.User;

namespace SAAD2.Views
{
    public partial class LoginPage : ContentPage
    {
        private const string FirebaseApiKey = "AIzaSyCW4PQCcScohZJTo4IfevkCRxxXbmQY7HA";
        private readonly FirebaseAuthClient client;

        public LoginPage()
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
            UpdateThemeIcon();

            var biometricEnabled = Preferences.Get("BiometricEnabled", false);
            if (biometricEnabled)
            {
                // Pequeno atraso para garantir que a UI est� pronta
                await Task.Delay(250);
            }
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                await DisplayAlert("Erro", "Usu�rio e senha s�o obrigat�rios.", "OK");
                return;
            }

            LoginButton.IsEnabled = false;
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                var userCredential = await client.SignInWithEmailAndPasswordAsync(UsernameEntry.Text, PasswordEntry.Text);
                if (userCredential != null && !string.IsNullOrEmpty(userCredential.User.Uid))
                {
                    // Salva o ID e o email do usu�rio para uso futuro
                    Preferences.Set("UserUid", userCredential.User.Uid);

                    // Use the email from the entry field, not the Firebase user object
                    Preferences.Set("UserEmail", UsernameEntry.Text);

                    await PromptToEnableBiometricsAsync();
                    Preferences.Set("IsLoggedIn", true);
                    await Shell.Current.GoToAsync("//main"); // Rota da TabBar principal
                }
            }
            catch (FirebaseAuthException)
            {
                await DisplayAlert("Erro de Login", "Credenciais inv�lidas.", "OK");
            }
            finally
            {
                LoginButton.IsEnabled = true;
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private async Task PromptToEnableBiometricsAsync()
        {
            bool enableBiometrics = await DisplayAlert("Login R�pido", "Deseja habilitar o login com biometria (Facial/Digital) para o pr�ximo acesso?", "Sim", "N�o");
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

        private void OnThemeIconTapped(object sender, TappedEventArgs e)
        {
            var app = (App)Application.Current;
            var novoTema = app.CurrentTheme == Theme.Light ? Theme.Dark : Theme.Light;
            app.SetTheme(novoTema);
            UpdateThemeIcon();
        }

        private void UpdateThemeIcon()
        {
            var app = (App)Application.Current;
            ThemeIconLabel.Text = app.CurrentTheme == Theme.Light
                ? FontAwesomeIcons.Sun
                : FontAwesomeIcons.Moon;
        }

        private async void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RecuperarSenhaPage));
        }

        private async void OnRegisterTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RegistroPage));
        }
    }
}

