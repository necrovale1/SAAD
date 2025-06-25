using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
// Adicione esta diretiva using
using Microsoft.Maui.Dispatching;

namespace SAAD
{
    public partial class LogoutPage : ContentPage
    {
        public LogoutPage()
        {
            InitializeComponent();
        }

        // CORREÇÃO: Altere o método para não ser 'async' e use o MainThread.
        private void btnConfirmLogout_Clicked(object sender, System.EventArgs e)
        {
            // Perform logout
            Preferences.Set("UsuarioLogado", false);

            // Navigate back to MainPage using the main thread
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
                }
            });
        }

        private async void btnCancelLogout_Clicked(object sender, System.EventArgs e)
        {
            // Go back to previous page
            await Shell.Current.GoToAsync("..");
        }
    }
}