using Firebase.Database; 
using SAAD2.Models;
using Firebase.Database.Query;
using System.Linq; // Adicionado para a função FirstOrDefault()

namespace SAAD2.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly FirebaseClient firebaseClient = new FirebaseClient("https://saad-1fd38-default-rtdb.firebaseio.com/");
        private string userUid;

        public HomePage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            userUid = Preferences.Get("UserUid", string.Empty);

            if (!string.IsNullOrEmpty(userUid))
            {
                await LoadUserName();
            }
        }

        private async Task LoadUserName()
        {
            try
            {
                var user = await firebaseClient.Child("Users").Child(userUid).OnceSingleAsync<User>();
                if (user != null && !string.IsNullOrEmpty(user.Nome))
                {
                    // Extrai o primeiro nome para uma saudação mais pessoal
                    var primeiroNome = user.Nome.Split(' ').FirstOrDefault();
                    WelcomeLabel.Text = $"Bem-vindo(a), {primeiroNome}!";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar nome do usuário: {ex.Message}");
                WelcomeLabel.Text = "Bem-vindo(a)!";
            }
        }

        // ADICIONE ESTE MÉTODO para navegar para a página de configurações
        private async void OnConfigClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(UserConfigPage));
        }

        // ADICIONE ESTE MÉTODO para navegar para a página de matérias
        private async void OnMateriasClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(MateriasPage));
        }

        // ADICIONE ESTE MÉTODO para navegar para a página de faltas
        private async void OnFaltasClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(FaltasPage));
        }

        // Este método já deve existir da nossa alteração anterior
        private async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            Preferences.Clear();
            SecureStorage.Default.RemoveAll();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}