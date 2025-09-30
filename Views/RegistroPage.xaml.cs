using Firebase.Auth;
using Firebase.Auth.Providers;

namespace SAAD2.Views
{
    public partial class RegistroPage : ContentPage
    {
        private const string FirebaseApiKey = "AIzaSyCW4PQCcScohZJTo4IfevkCRxxXbmQY7HA";
        private readonly FirebaseAuthClient client;

        public RegistroPage()
        {
            InitializeComponent();
            client = new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = FirebaseApiKey,
                AuthDomain = "saad-1fd38.firebaseapp.com",
                Providers = new FirebaseAuthProvider[] { new EmailProvider() }
            });
        }

        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameEntry.Text) ||
                string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
                string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
            {
                await DisplayAlert("Erro", "Todos os campos são obrigatórios.", "OK");
                return;
            }

            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                await DisplayAlert("Erro", "As senhas não coincidem.", "OK");
                return;
            }

            try
            {
                var userCredential = await client.CreateUserWithEmailAndPasswordAsync(EmailEntry.Text, PasswordEntry.Text, UsernameEntry.Text);
                await DisplayAlert("Sucesso", "Usuário registrado com sucesso!", "OK");
                await Shell.Current.GoToAsync(".."); // Volta para a página de login
            }
            catch (FirebaseAuthException ex)
            {
                var friendlyMessage = ex.Reason switch
                {
                    AuthErrorReason.EmailExists => "Este e-mail já está em uso.",
                    AuthErrorReason.WeakPassword => "A senha é muito fraca. Tente uma mais forte.",
                    _ => "Ocorreu um erro durante o registro."
                };
                await DisplayAlert("Erro de Registro", friendlyMessage, "OK");
            }
        }

        private async void OnVoltarClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}