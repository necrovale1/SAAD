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

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!Preferences.Get("UsuarioLogado", false))
            {
                await DisplayAlert("Aviso", "Você precisa fazer login", "OK");

                // CORREÇÃO: Adicionada verificação para evitar erro de referência nula.
                if (Shell.Current != null)
                {
                    await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
                }
            }
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