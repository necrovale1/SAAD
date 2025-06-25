using SAAD2.Services;
using SAAD2.Views;
using SAAD2.Helpers;
using SAAD2.Models;


namespace SAAD2.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private async void OnMateriasClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("MateriasPage");
        }

        private async void OnFaltasClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("FaltasPage");
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("LogoutPage");
        }
    }
}