using SAAD2.Models;
using SAAD2.Services;

namespace SAAD2.Views
{
    [QueryProperty(nameof(FaltaToEdit), "SelectedFalta")]
    public partial class RegistroFaltasPage : ContentPage
    {
        private Falta _faltaToEdit;
        public Falta FaltaToEdit
        {
            get => _faltaToEdit;
            set
            {
                _faltaToEdit = value;
                if (_faltaToEdit != null)
                {
                    PreencherCampos();
                }
            }
        }
        public RegistroFaltasPage()
        {
            InitializeComponent();
        }

        private void PreencherCampos()
        {
            Title = "Editar Registro de Faltas";
            SaveButton.Text = "Salvar Alterações";

            MateriaEntry.Text = FaltaToEdit.Materia;
            FaltasEntry.Text = FaltaToEdit.Faltas.ToString();
            PresencasEntry.Text = FaltaToEdit.Presencas.ToString();
        }

        private async void OnSalvarClicked(object sender, EventArgs e)
        {
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

            SaveButton.IsEnabled = false;
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                if (FaltaToEdit == null)
                {
                    // MODO CRIAÇÃO
                    var novaFalta = new Falta
                    {
                        Materia = MateriaEntry.Text,
                        Faltas = faltas,
                        Presencas = presencas
                    };
                    await FaltaService.Instance.AddFaltaAsync(novaFalta);
                    await DisplayAlert("Sucesso", "Falta registrada com sucesso!", "OK");
                }
                else
                {
                    // MODO EDIÇÃO
                    FaltaToEdit.Materia = MateriaEntry.Text;
                    FaltaToEdit.Faltas = faltas;
                    FaltaToEdit.Presencas = presencas;
                    await FaltaService.Instance.UpdateFaltaAsync(FaltaToEdit);
                    await DisplayAlert("Sucesso", "Registro de faltas atualizado com sucesso!", "OK");
                }

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Ocorreu um erro ao salvar: {ex.Message}", "OK");
            }
            finally
            {
                SaveButton.IsEnabled = true;
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }
    }
}