using SAAD2.Models;
using SAAD2.Services;

namespace SAAD2.Views
{
    public partial class RegistroMateriasPage : ContentPage
    {
        public RegistroMateriasPage()
        {
            InitializeComponent();
        }

        private async void OnSalvarClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NomeEntry.Text) ||
                string.IsNullOrWhiteSpace(DescricaoEntry.Text) ||
                string.IsNullOrWhiteSpace(CategoriaEntry.Text))
            {
                await DisplayAlert("Erro", "Todos os campos são obrigatórios.", "OK");
                return;
            }

            var novaMateria = new Materia
            {
                Nome = NomeEntry.Text,
                Descricao = DescricaoEntry.Text,
                Categoria = CategoriaEntry.Text
            };

            await MateriaService.Instance.AddMateriaAsync(novaMateria);
            await Shell.Current.GoToAsync("..");
        }
    }
}