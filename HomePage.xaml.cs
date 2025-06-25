using Microsoft.Maui.Controls;
using System.Web;

namespace SAAD
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private async void btnMaterias_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(MateriasPage));
        }

        // Dentro da classe HomePage.xaml.cs
        private async void btnFaltas_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(FaltasPage));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(LogoutPage));
        }
    }
}