using Microsoft.Maui.Controls;
using System;

namespace SAAD
{
    public partial class RecuperarSenha : ContentPage
    {
        public RecuperarSenha()
        {
            InitializeComponent();
        }

        private async void OnSendRecoveryLinkClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text?.Trim() ?? string.Empty;

            if (!IsValidEmail(email))
            {
                await DisplayAlert("Erro", "Por favor, insira um e-mail válido", "OK");
                return;
            }

            // Show loading indicator
            await DisplayAlert("Link Enviado",
                $"Um link de recuperação foi enviado para {email}. Verifique sua caixa de entrada.",
                "OK");

            await Navigation.PopAsync();
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async void OnLoginTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}