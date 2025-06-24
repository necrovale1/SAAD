using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace SAAD
{
    public partial class LogoutPage : ContentPage
    {
        public LogoutPage()
        {
            InitializeComponent();
        }

        private async void btnConfirmLogout_Clicked(object sender, System.EventArgs e)
        {
            // Perform logout
            Preferences.Set("UsuarioLogado", false);

            // Navigate back to MainPage
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }

        private async void btnCancelLogout_Clicked(object sender, System.EventArgs e)
        {
            // Go back to previous page
            await Shell.Current.GoToAsync("..");
        }
    }
}