namespace SAAD
{
    public partial class MainPage : ContentPage
    {
        private const string ValidUsername = "aluno";
        private const string ValidPassword = "2025";

        public MainPage()
        {
            InitializeComponent();
        }

        private async void LoginVerify(object sender, EventArgs e)
        {
            string username = Username.Text?.Trim() ?? string.Empty;
            string password = Password.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos", "OK");
                return;
            }

            if (username == ValidUsername && password == ValidPassword)
            {
                Preferences.Set("UsuarioLogado", true);

                // Navega para a HomePage usando o sistema de rotas do Shell.
                // O "//" garante que a pilha de navegação seja resetada.
                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            }
            else
            {
                await DisplayAlert("Erro", "Credenciais inválidas", "OK");
            }
        }

        private async void RecuperarSenha(object sender, EventArgs e)
        {
            // A navegação para páginas de recuperação e registro agora funciona corretamente.
            await Shell.Current.GoToAsync(nameof(RecuperarSenha));
        }

        private async void Registrar(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Registro));
        }
    }
}