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
            await Task.Delay(3000); // Aguarda 3 segundos

            // Acede à primeira (e única) janela da aplicação e define a sua página.
            if (Application.Current != null)
                Application.Current.Windows[0].Page = new AppShell();
        }
    }
}