using SAAD.Resources.Styles;
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
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
        }

        private async void btnMaterias_Clicked(object sender, EventArgs e)
        {
            try
            {
                var materias = new Materias
                {
                    Nome = "Programação Web",
                    Descricao = "Programação web é o processo de desenvolvimento...",
                    Categoria = "Web"
                };

                // Fix: Removed usage of inaccessible 'ShellNavigationParameters' and replaced with query parameters
                var queryParams = new Dictionary<string, object>
                    {
                        { "MateriaData", materias }
                    };
                await Shell.Current.GoToAsync($"//{nameof(MateriasPage)}", queryParams);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Não foi possível acessar as matérias: {ex.Message}", "OK");
            }
        }

        private async void btnFaltas_Clicked(object sender, EventArgs e)
        {
            try
            {
                var faltas = new Faltas
                {
                    Materia = "Programação Web",
                    Falta = 5,
                    Presenca = 20
                };

                // Using absolute route with query parameters
                await Shell.Current.GoToAsync(
                    $"//{nameof(FaltasPage)}?" +
                    $"Materia={HttpUtility.UrlEncode(faltas.Materia)}&" +
                    $"Falta={faltas.Falta}&" +
                    $"Presenca={faltas.Presenca}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Não foi possível acessar as faltas: {ex.Message}", "OK");
            }
        }
    }
}