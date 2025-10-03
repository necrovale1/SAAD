using Firebase.Auth;
using Firebase.Auth.Providers;
using SAAD2.Views; // Garante que as outras páginas da pasta Views sejam encontradas

namespace SAAD2;

public partial class MainPage : ContentPage
{
    // Sua Web API Key do Firebase
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

            // --- LÓGICA DE AUTENTICAÇÃO REAL ---
            var config = new FirebaseConfig(FirebaseWebAppKey);
            var authProvider = new FirebaseAuthProvider(config);

            // Tenta fazer o login com o email e senha fornecidos
            var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, senha);

            // Se a linha acima não gerou um erro, o login foi bem-sucedido.
            // Agora, navega para a página de verificação facial.
            // Usamos PushAsync porque a MainPage foi envolvida em um NavigationPage
            await Navigation.PushAsync(new FaceAuthPage());

        }
        catch (FirebaseAuthException ex)
        {
            // Trata erros específicos do Firebase, como senha errada ou usuário não encontrado
            await DisplayAlert("Erro de Login", "Email ou senha inválidos. Por favor, tente novamente.", "OK");
        }
        catch (Exception ex)
        {
            // Trata outros erros genéricos (como falta de conexão com a internet)
            System.Diagnostics.Debug.WriteLine($"Erro inesperado no login: {ex}");
            await DisplayAlert("Erro", "Ocorreu um problema. Verifique sua conexão com a internet e tente novamente.", "OK");
        }
    }

    private async void OnRegistroClicked(object sender, EventArgs e)
    {
        // Navega para a página de registro de novo usuário
        await Navigation.PushAsync(new RegistroPage());
    }

    private async void OnRecuperarSenhaClicked(object sender, EventArgs e)
    {
        // Navega para a página de recuperação de senha
        await Navigation.PushAsync(new RecuperarSenhaPage());
    }
}