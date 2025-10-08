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

            // Acede � primeira (e �nica) janela da aplica��o e define a sua p�gina.
            if (Application.Current != null)
                Application.Current.Windows[0].Page = new AppShell();
        }
    }
}