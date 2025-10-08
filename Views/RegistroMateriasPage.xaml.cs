using SAAD2.Models;
using SAAD2.Services;


namespace SAAD2.Views
{

    [QueryProperty(nameof(MateriaToEdit), "SelectedMateria")]

    public partial class RegistroMateriasPage : ContentPage
    {
        private readonly MateriaService _materiaService;
        private Materia _materiaToEdit;

        public Materia MateriaToEdit
        {
            get => _materiaToEdit;
            set
            {
                _materiaToEdit = value;
                if (_materiaToEdit != null)
                {
                    PreencherCampos();
                }
            }
        }

        public RegistroMateriasPage()
        {
            InitializeComponent();
            _materiaService = MateriaService.Instance;
        }

        private void PreencherCampos()
        {
            Title = "Editar Matéria";
            SaveButton.Text = "Salvar Alterações";
            NomeMateriaEntry.Text = MateriaToEdit.Nome;
            ProfessorEntry.Text = MateriaToEdit.Professor;
            CategoriaEntry.Text = MateriaToEdit.Categoria;
        }

        private async void OnSaveMateriaClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NomeMateriaEntry.Text))
            {
                await DisplayAlert("Campo Obrigatório", "O nome da matéria é obrigatório.", "OK");
                return;
            }

            SaveButton.IsEnabled = false;
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                if (MateriaToEdit == null)
                {
                    var novaMateria = new Materia
                    {
                        Nome = NomeMateriaEntry.Text,
                        Professor = ProfessorEntry.Text,
                        Categoria = CategoriaEntry.Text
                    };
                    await _materiaService.AddMateriaAsync(novaMateria);
                    await DisplayAlert("Sucesso", "Matéria registrada com sucesso!", "OK");
                }
                else
                {
                    MateriaToEdit.Nome = NomeMateriaEntry.Text;
                    MateriaToEdit.Professor = ProfessorEntry.Text;
                    MateriaToEdit.Categoria = CategoriaEntry.Text;
                    await _materiaService.UpdateMateriaAsync(MateriaToEdit);
                    await DisplayAlert("Sucesso", "Matéria atualizada com sucesso!", "OK");
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