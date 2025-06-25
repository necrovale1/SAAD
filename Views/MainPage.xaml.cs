using SAAD2.Services;
using SAAD2.Views;
using SAAD2.Helpers;
using SAAD2.Models;

namespace SAAD2.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                await DisplayAlert("Erro", "Usuário e senha são obrigatórios.", "OK");
                return;
            }

            if (UsernameEntry.Text == "aluno" && PasswordEntry.Text == "2025")
            {
                Preferences.Set("IsLoggedIn", true);
                await Shell.Current.GoToAsync("//HomePage");
            }
            else
            {
                await DisplayAlert("Erro", "Credenciais inválidas.", "OK");
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