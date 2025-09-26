using SAAD2.Views;
using Firebase.Auth;
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
                AuthDomain = "saad-1fd38.firebaseapp.com"
            });
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
                    Preferences.Set("IsLoggedIn", true);
                    await Shell.Current.GoToAsync("//HomePage");
                }
            }
            catch (FirebaseAuthException ex)
            {
                // Mensagem de erro mais amigável para o usuário
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

        private async void OnBiometricLoginButtonClicked(object sender, EventArgs e)
        {
            var result = await BiometricAuthenticationService.Default.AuthenticateAsync(
                new AuthenticationRequest
                {
                    Title = "Login Biométrico",
                    NegativeText = "Cancelar"
                }, CancellationToken.None);

            if (result.Status == BiometricResponseStatus.Success)
            {
                Preferences.Set("IsLoggedIn", true);
                await Shell.Current.GoToAsync("//HomePage");
            }
            else
            {
                await DisplayAlert("Erro", "Autenticação biométrica falhou.", "OK");
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