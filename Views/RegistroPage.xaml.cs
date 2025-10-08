// --- USINGS NECESSÁRIOS ---
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Firebase.Database.Query;
using SAAD2.Models;
// Alias para resolver o conflito entre os dois tipos de 'User'
using UserModel = SAAD2.Models.User;

namespace SAAD2.Views
{
    public partial class RegistroPage : ContentPage
    {
        // --- DECLARAÇÃO DOS CLIENTES FIREBASE ---
        private readonly FirebaseAuthClient client;
        private readonly FirebaseClient firebaseClient;

        public RegistroPage()
        {
            InitializeComponent();

            // --- INICIALIZAÇÃO DOS CLIENTES FIREBASE ---
            client = new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = "AIzaSyCW4PQCcScohZJTo4IfevkCRxxXbmQY7HA", // A sua chave de API
                AuthDomain = "saad-1fd38.firebaseapp.com",
                Providers = new FirebaseAuthProvider[] { new EmailProvider() }
            });

            firebaseClient = new FirebaseClient("https://saad-1fd38-default-rtdb.firebaseio.com/");
        }

        // --- LÓGICA DO BOTÃO CORRIGIDA ---
        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NomeEntry.Text) ||
                string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
                string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
            {
                await DisplayAlert("Erro", "Todos os campos são obrigatórios.", "OK");
                return;
            }

            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                await DisplayAlert("Erro", "As senhas não conferem.", "OK");
                return;
            }

            RegisterButton.IsEnabled = false;
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                // Cria o utilizador na autenticação
                var userCredential = await client.CreateUserWithEmailAndPasswordAsync(EmailEntry.Text, PasswordEntry.Text);

                // A variável 'user' vem de 'userCredential.User'
                var user = userCredential.User;

                if (user != null)
                {
                    // Usando o alias 'UserModel' para criar o perfil na base de dados
                    var userProfile = new UserModel
                    {
                        Nome = NomeEntry.Text,
                        Email = EmailEntry.Text,
                        UserType = "Aluno" // Define o tipo de utilizador como "Aluno" por defeito
                    };

                    // Guarda o perfil na base de dados
                    await firebaseClient
                        .Child("Users")
                        .Child(user.Uid)
                        .PutAsync(userProfile);

                    await DisplayAlert("Sucesso", "Sua conta foi criada!", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.Reason == AuthErrorReason.EmailExists)
                {
                    await DisplayAlert("Erro de Registro", "Este email já está em uso.", "OK");
                }
                else
                {
                    await DisplayAlert("Erro de Registro", "Ocorreu um erro: " + ex.Message, "OK");
                }
            }
            finally
            {
                RegisterButton.IsEnabled = true;
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private async void OnLoginTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}