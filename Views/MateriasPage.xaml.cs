using SAAD2.Services;


namespace SAAD2.Views
{
    public partial class MateriasPage : ContentPage
    {
        public MateriasPage()
        {
            InitializeComponent();
            BindingContext = MateriaService.Instance;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MateriasListView.ItemsSource = MateriaService.Instance.Materias;
        }

        private async void OnRegistrarClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("RegistroMateriasPage");
        }
    }
}