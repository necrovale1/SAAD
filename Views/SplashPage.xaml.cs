// Views/SplashPage.xaml.cs
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

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
            // Inicia a animação ao aparecer
            _ = AnimateLogo();
            await Task.Delay(2000); // 2 segundos de splash
            Application.Current.MainPage = new AppShell();
        }

        private async Task AnimateLogo()
        {
            if (LogoImage == null)
                return;

            while (true)
            {
                await LogoImage.RotateTo(360, 1000);
                LogoImage.Rotation = 0;
            }
        }
    }
}