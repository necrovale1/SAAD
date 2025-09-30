using SAAD2.Models;
using SAAD2.Services;

namespace SAAD2.Views
{
    public partial class RegistroFaltasPage : ContentPage
    {
        public RegistroFaltasPage()
        {
            InitializeComponent();
        }

        private async void OnSalvarClicked(object sender, EventArgs e)
        {
            // Validação simples
            if (string.IsNullOrWhiteSpace(MateriaEntry.Text) ||
                string.IsNullOrWhiteSpace(FaltasEntry.Text) ||
                string.IsNullOrWhiteSpace(PresencasEntry.Text))
            {
                await DisplayAlert("Erro", "Todos os campos são obrigatórios.", "OK");
                return;
            }

            if (!int.TryParse(FaltasEntry.Text, out int faltas) || !int.TryParse(PresencasEntry.Text, out int presencas))
            {
                await DisplayAlert("Erro", "Os campos de faltas e presenças devem ser números.", "OK");
                return;
            }

            var novaFalta = new Falta
            {
                Materia = MateriaEntry.Text,
                Faltas = faltas,
                Presencas = presencas
            };

            await FaltaService.Instance.AddFaltaAsync(novaFalta);
            await Shell.Current.GoToAsync("..");
        }
    }
}