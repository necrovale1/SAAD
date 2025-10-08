namespace SAAD2.Views
{
    public partial class LogoutPage : ContentPage
    {
        public LogoutPage()
        {
            InitializeComponent();
        }

        private async void OnConfirmLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Set("IsLoggedIn", false);
            // O '//' antes de MainPage reseta a pilha de navega��o, enviando o usu�rio para a raiz (Login)
            await Shell.Current.GoToAsync("//MainPage");
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(".."); // Apenas volta para a p�gina anterior
        }

        private  void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UserConfigPage());
        }
    }
}