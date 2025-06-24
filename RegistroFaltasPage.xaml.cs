using SAAD.Services;

namespace SAAD;

public partial class RegistroFaltasPage : ContentPage
{
    public RegistroFaltasPage()
    {
        InitializeComponent();
    }

    private async void OnSalvarClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(MateriaEntry.Text) ||
            !int.TryParse(FaltasEntry.Text, out int faltas) ||
            !int.TryParse(PresencasEntry.Text, out int presencas))
        {
            await DisplayAlert("Erro", "Por favor, preencha todos os campos corretamente.", "OK");
            return;
        }

        var novaFalta = new Faltas
        {
            Materia = MateriaEntry.Text,
            Falta = faltas,
            Presenca = presencas
        };

        FaltaService.Instance.AddFalta(novaFalta);
        await DisplayAlert("Sucesso", "Registro de faltas salvo!", "OK");
        await Shell.Current.GoToAsync("..");
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}