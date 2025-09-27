using SAAD2.Views;
using Firebase.Auth;
using Firebase.Auth.Providers;
using System.Threading;
using Plugin.Maui.Biometric;

namespace SAAD2.Views
{
    public partial class MainPage : ContentPage
    {
        private const string FirebaseApiKey = "AIzaSyCW4PQCcScohZJTo4IfevkCRxxXbmQY7HA";
        private readonly FirebaseAuthClient client;

        public MainPage()
        {
            InitializeComponent();

            client = new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = FirebaseApiKey,
                AuthDomain = "saad-1fd38.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            });
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var biometricEnabled = Preferences.Get("BiometricEnabled", false);
            if (biometricEnabled)
            {
                await Task.Delay(250);
                await PerformBiometricLoginAsync(true); // true = login automático
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
                    await PromptToEnableBiometricsAsync();

                    Preferences.Set("IsLoggedIn", true);
                    await Shell.Current.GoToAsync("//HomePage");
                }
            }
            catch (FirebaseAuthException ex)
            {
                var friendlyMessage = ex.Reason switch
                {
                    AuthErrorReason.InvalidEmailAddress => "O formato do e-mail é inválido.",
                    AuthErrorReason.WrongPassword => "A senha está incorreta.",
                    AuthErrorReason.UserNotFound => "Usuário não encontrado.",
                    _ => "Ocorreu um erro de login. Verifique suas credenciais."
                };
                await DisplayAlert("Erro de Login", friendlyMessage, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
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

        private async void OnBiometricLoginButtonClicked(object sender, EventArgs e)
        {
            await PerformBiometricLoginAsync(false); // false = clique no botão
        }

        private async Task PerformBiometricLoginAsync(bool isAutomatic)
        {
            var request = new AuthenticationRequest
            {
                Title = "Login Biométrico",
                // A propriedade Subtitle não existe na v0.0.4, então a removemos.
                NegativeText = "Cancelar"
            };

            var result = await BiometricAuthenticationService.Default.AuthenticateAsync(request, CancellationToken.None);

            // A única verificação possível e necessária é o sucesso.
            if (result.Status == BiometricResponseStatus.Success)
            {
                try
                {
                    var email = await SecureStorage.Default.GetAsync("auth_email");
                    var password = await SecureStorage.Default.GetAsync("auth_password");

                    if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                    {
                        var userCredential = await client.SignInWithEmailAndPasswordAsync(email, password);
                        if (userCredential != null && !string.IsNullOrEmpty(userCredential.User.Uid))
                        {
                            Preferences.Set("IsLoggedIn", true);
                            await Shell.Current.GoToAsync("//HomePage");
                        }
                    }
                    else if (!isAutomatic)
                    {
                        await DisplayAlert("Erro", "Credenciais não encontradas. Faça login com usuário e senha primeiro para habilitar.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    if (!isAutomatic)
                    {
                        await DisplayAlert("Erro", $"Falha no login automático: {ex.Message}", "OK");
                    }
                }
            }
            else if (!isAutomatic)
            {
                // Qualquer status que não seja 'Success' será tratado como uma falha geral.
                await DisplayAlert("Falha", "Autenticação biométrica não disponível, falhou ou foi cancelada.", "OK");
            }
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