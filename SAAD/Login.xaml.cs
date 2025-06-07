using Microsoft.Maui.Controls;

namespace SAAD
{
    public partial class Login : ContentPage
    {
        // Hardcoded credentials (for simple demo only)
        private const string ValidUsername = "aluno";
        private const string ValidPassword = "2025";

        public Login()
        {
            InitializeComponent();
        }

        private void LoginVerify(object sender, EventArgs e)
        {
            string username = Username.Text?.Trim();
            string password = Password.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                DisplayAlert("Erro", "Por favor, insira o nome de usuário", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                DisplayAlert("Erro", "Por favor, insira a senha", "OK");
                return;
            }

            if (username == ValidUsername && password == ValidPassword)
            {
                // Successful login - navigate to main page
                DisplayAlert("Sucesso", "Login bem-sucedido!", "OK");
                // Navigation.PushAsync(new MainPage()); // Uncomment when you have a MainPage
            }
            else
            {
                DisplayAlert("Erro", "Usuário ou senha incorretos", "OK");
                Password.Text = string.Empty; // Clear password field
            }
        }

        private async void RecuperarSenha(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RecuperarSenha());
        }

        private async void Registrar(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new Registro());
        }
    }
}