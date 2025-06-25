using SAAD.Services;
using SAAD.Models;

namespace SAAD;

public partial class RegistroMateriasPage : ContentPage
{
    public RegistroMateriasPage()
    {
        InitializeComponent();
    }

    private async void OnSalvarClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NomeEntry.Text) ||
            string.IsNullOrWhiteSpace(DescricaoEditor.Text) ||
            string.IsNullOrWhiteSpace(CategoriaEntry.Text))
        {
            await DisplayAlert("Erro", "Todos os campos são obrigatórios.", "OK");
            return;
        }

        var novaMateria = new Materias
        {
            Nome = NomeEntry.Text,
            Descricao = DescricaoEditor.Text,
            Categoria = CategoriaEntry.Text
        };

        MateriaService.Instance.AddMateria(novaMateria);

        await DisplayAlert("Sucesso", "Matéria registrada!", "OK");
        await Shell.Current.GoToAsync("..");
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}