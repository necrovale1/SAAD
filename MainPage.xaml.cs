using SAAD.Resources.Styles;
using Microsoft.Maui.Controls;

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
            _isDarkTheme = Preferences.Get("DarkTheme", false);
            ApplyTheme(_isDarkTheme);
            UpdateThemeButton();
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
                await DisplayAlert("Sucesso", "Login bem-sucedido!", "OK");

                // Navigate to AppShell
                if (Application.Current != null)
                {
                    Application.Current.MainPage = new AppShell();
                }
            }
            else
            {
                await DisplayAlert("Erro", "Credenciais inválidas", "OK");
            }
        }

        private async void RecuperarSenha(object sender, EventArgs e)
        {
            try
            {
                var shell = Shell.Current;
                if (shell != null)
                {
                    await shell.GoToAsync($"//{nameof(RecuperarSenha)}");
                }
                else if (Application.Current != null)
                {
                    Application.Current.MainPage = new AppShell();
                    shell = Shell.Current;
                    if (shell != null)
                    {
                        await shell.GoToAsync($"//{nameof(RecuperarSenha)}");
                    }
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
                var shell = Shell.Current;
                if (shell != null)
                {
                    await shell.GoToAsync($"//{nameof(Registro)}");
                }
                else if (Application.Current != null)
                {
                    Application.Current.MainPage = new AppShell();
                    shell = Shell.Current;
                    if (shell != null)
                    {
                        await shell.GoToAsync($"//{nameof(Registro)}");
                    }
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
            ApplyTheme(_isDarkTheme);
            UpdateThemeButton();
            Preferences.Set("DarkTheme", _isDarkTheme);
        }
    }
}