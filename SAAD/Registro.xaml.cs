using Microsoft.Maui.Controls;

namespace SAAD
{
    public partial class Registro : ContentPage
    {
        public Registro()
        {
            InitializeComponent();
        }

        private async void Registrar(object sender, EventArgs e)
        {
            string name = TrimmedEntryText(NameEntry);
            string email = TrimmedEntryText(EmailEntry);
            string password = EntryText(PasswordEntry);
            string confirmPassword = EntryText(ConfirmPasswordEntry);

            // Valida��o
            if (string.IsNullOrWhiteSpace(name))
            {
                await DisplayAlert("Erro", "Por favor, insira seu nome completo.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                await DisplayAlert("Erro", "Por favor, insira um e-mail v�lido.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                await DisplayAlert("Erro", "A senha deve ter pelo menos 6 caracteres.", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Erro", "As senhas n�o coincidem.", "OK");
                return;
            }

            // L�gica de registro (exemplo)
            await DisplayAlert("Sucesso", "Cadastro realizado com sucesso!", "OK");

            // Redirecionamento opcional para tela de login
            // await Navigation.PushAsync(new Login());
        }

        private async void OnLoginTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Login());
        }

        // M�todos auxiliares para evitar erros de acesso a Entry.Text
        private string TrimmedEntryText(Entry entry)
        {
            return entry?.Text?.Trim() ?? string.Empty;
        }

        private string EntryText(Entry entry)
        {
            return entry?.Text ?? string.Empty;
        }
    }
}
