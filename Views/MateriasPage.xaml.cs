using SAAD2.Models;
using SAAD2.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SAAD2.Views
{
    public partial class MateriasPage : ContentPage
    {
        private readonly MateriaService _materiaService;
        public ICommand DeleteCommand { get; }
        public bool IsSchoolUser { get; private set; }

        public MateriasPage()
        {
            InitializeComponent();
            _materiaService = MateriaService.Instance;

            DeleteCommand = new Command<Materia>(async (materia) => await ExecuteDeleteCommand(materia));
            this.BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            LoadingIndicator.IsVisible = true;
            MateriasListView.IsVisible = false;

            var userType = Preferences.Get("UserType", string.Empty);
            IsSchoolUser = (userType == "Escola");
            OnPropertyChanged(nameof(IsSchoolUser));

            // Agora o nome BtnRegistrarMateria vai ser encontrado corretamente
            BtnRegistrarMateria.IsVisible = IsSchoolUser;

            await _materiaService.LoadMateriasAsync();
            MateriasListView.ItemsSource = _materiaService.Materias;

            LoadingIndicator.IsVisible = false;
            MateriasListView.IsVisible = true;
        }

        private async Task ExecuteDeleteCommand(Materia materia)
        {
            bool confirm = await DisplayAlert("Confirmar Exclusão", $"Tem certeza de que deseja excluir a matéria '{materia.Nome}'?", "Sim", "Não");
            if (confirm)
            {
                await _materiaService.DeleteMateriaAsync(materia);
            }
        }

        private async void OnRegistrarClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RegistroMateriasPage));
        }
    }
}