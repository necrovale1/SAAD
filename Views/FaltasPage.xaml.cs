using SAAD2.Models;
using SAAD2.Services;
using System.Collections.ObjectModel;
using System.Windows.Input; 

namespace SAAD2.Views
{
    public partial class FaltasPage : ContentPage
    {
        private readonly FaltaService _faltaService;
        public ICommand DeleteCommand { get; }

        public ICommand EditCommand { get; }
        public bool IsProfessorUser { get; private set; }
        public ObservableCollection<Falta> Faltas { get; set; }

        public FaltasPage()
        {
            InitializeComponent();
            _faltaService = FaltaService.Instance;
            Faltas = new ObservableCollection<Falta>();

            DeleteCommand = new Command<Falta>(async (falta) => await ExecuteDeleteCommand(falta));
            EditCommand = new Command<Falta>(async (falta) => await ExecuteEditCommand(falta));

            this.BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            LoadingIndicator.IsVisible = true;
            FaltasListView.IsVisible = false;

            var userType = Preferences.Get("UserType", string.Empty);
            IsProfessorUser = (userType == "Professor");
            OnPropertyChanged(nameof(IsProfessorUser));

            BtnRegistrarFalta.IsVisible = IsProfessorUser;

            await _faltaService.LoadFaltasAsync();
            FaltasListView.ItemsSource = _faltaService.Faltas;

            LoadingIndicator.IsVisible = false;
            FaltasListView.IsVisible = true;
        }

        private async Task ExecuteDeleteCommand(Falta falta)
        {
            bool confirm = await DisplayAlert("Confirmar Exclusão", $"Tem certeza de que deseja excluir o registro de faltas da matéria '{falta.Materia}'?", "Sim", "Não");
            if (confirm)
            {
                await _faltaService.DeleteFaltaAsync(falta);
            }
        }
        private async Task ExecuteEditCommand(Falta falta)
        {
            // Navega para a página de registo, enviando o objeto falta
            await Shell.Current.GoToAsync($"{nameof(RegistroFaltasPage)}", new Dictionary<string, object>
        {
            { "SelectedFalta", falta }
        });
        }
        private async void OnRegistrarFaltaClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RegistroFaltasPage));
        }
    }
}