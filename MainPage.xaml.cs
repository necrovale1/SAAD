using SAAD.Resources.Styles;

namespace SAAD
{
    public partial class MainPage : ContentPage
    {
        private const string ValidUsername = "aluno";
        private const string ValidPassword = "2025";
        private bool _isDarkTheme = false;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void LoginVerify(object sender, EventArgs e)
        {
            string username = Username.Text?.Trim() ?? string.Empty;
            string password = Password.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(username))
            {
                await DisplayAlert("Erro", "Por favor, insira o nome de usuário", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Erro", "Por favor, insira a senha", "OK");
                return;
            }
            if (username == ValidUsername && password == ValidPassword)
            {
                Preferences.Set("UsuarioLogado", true);
                await DisplayAlert("Sucesso", "Login bem-sucedido!", "OK");

                if (Application.Current != null)
                {
                    // Apenas crie o AppShell. Ele automaticamente carregará a HomePage
                    // que foi definida como padrão no XAML.
                    Application.Current.MainPage = new AppShell();
                }
            }
        }
        private async void RecuperarSenha(object sender, EventArgs e)
        {
            try
            {
                // Verificação segura para Shell.Current
                if (Shell.Current != null)
                {
                    await Shell.Current.GoToAsync($"//{nameof(RecuperarSenha)}");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Não foi possível navegar: {ex.Message}", "OK");
            }
        }

        private async void Registrar(object sender, EventArgs e)
        {
            try
            {
                // Verificação segura para Shell.Current
                if (Shell.Current != null)
                {
                    await Shell.Current.GoToAsync($"//{nameof(Registro)}");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Não foi possível navegar: {ex.Message}", "OK");
            }
        }
        private void ApplyTheme(bool isDark)
        {
            if (Application.Current != null)
            {
                Application.Current.UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
            }
        }

        private void UpdateThemeButton()
        {
            if (ThemeToggleButton != null)
            {
                ThemeToggleButton.Text = _isDarkTheme ? "🌙" : "🌞";
            }
        }

        private void OnThemeToggleClicked(object sender, EventArgs e)
        {
            _isDarkTheme = !_isDarkTheme;
            ApplyTheme(_isDarkTheme); // Now properly called
            UpdateThemeButton();
            Preferences.Set("DarkTheme", _isDarkTheme); // Now the field is being used
        }
    }
}