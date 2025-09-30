// Views/SplashPage.xaml.cs
namespace SAAD2.Views
{
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Apenas espere o tempo do splash
            await Task.Delay(2000); // 2 segundos de splash
            // Navegue para a página principal
            Application.Current.MainPage = new AppShell();
        }
    }
}