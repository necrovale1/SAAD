using SAAD.Resources.Styles;

namespace SAAD
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Set SplashScreen as the main page
            MainPage = new SplashScreen();
        }
    
    }
}