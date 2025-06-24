namespace SAAD
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Inicia o aplicativo com a sua tela de splash animada
            MainPage = new SplashScreen();
        }
    }
}