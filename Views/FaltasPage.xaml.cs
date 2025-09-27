using SAAD2.Services;

namespace SAAD2.Views
{
    public partial class FaltasPage : ContentPage
    {
        public FaltasPage()
        {
            InitializeComponent();
            // Define o contexto de dados para a instância do serviço de faltas
            BindingContext = FaltaService.Instance;
        }

        // Garante que a lista seja atualizada sempre que a página aparecer
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await FaltaService.Instance.LoadFaltasAsync(); // Carrega os dados de forma assíncrona
                                                           // A linha abaixo não é mais necessária, pois o BindingContext já cuida disso
                                                           // (BindingContext as FaltaService).Faltas.CollectionChanged += (s, e) => { };
        }
        private async void OnRegistrarFaltaClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RegistroFaltasPage));
        }
    }
}