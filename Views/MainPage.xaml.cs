using Firebase.Auth;
using Firebase.Auth.Providers;
using SAAD2.Views;

namespace SAAD2;

public partial class MainPage : ContentPage
{
    private const string FirebaseWebAppKey = "AIzaSyCW4PQCcScohZJTo4IfevkCRxxXbmQY7HA";

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            string email = EmailEntry.Text?.Trim();
            string senha = SenhaEntry.Text?.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                await DisplayAlert("Erro", "Por favor, preencha o email e a senha.", "OK");
                return;
            }

            var config = new FirebaseConfig(FirebaseWebAppKey);
            var authProvider = new FirebaseAuthProvider(config);

            var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, senha);

            // A navegação aqui foi mantida como PushAsync, pois você ainda não tem um AppShell
            // quando o app é aberto pela primeira vez. Esta é a abordagem correta neste cenário.
            await Navigation.PushAsync(new FaceAuthPage());
        }
        catch (FirebaseAuthException firebaseEx) // Variável firebaseEx utilizada no Debug
        {
            System.Diagnostics.Debug.WriteLine($"Erro Firebase: {firebaseEx.Message}");
            await DisplayAlert("Erro de Login", "Email ou senha inválidos. Por favor, tente novamente.", "OK");
        }
        catch (Exception ex) // Variável ex utilizada no Debug
        {
            // Correção do aviso CS0168: Usando a variável 'ex' para registrar o erro.
            System.Diagnostics.Debug.WriteLine($"Erro inesperado no login: {ex}");
            await DisplayAlert("Erro", "Ocorreu um problema. Verifique sua conexão com a internet e tente novamente.", "OK");
        }
    }

    private async void OnRegistroClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistroPage());
    }

    private async void OnRecuperarSenhaClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RecuperarSenhaPage());
    }
}