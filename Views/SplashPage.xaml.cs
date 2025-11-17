using SAAD.Services;
using System.Threading.Tasks;

namespace SAAD.Views;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            // CORREÇÃO: Instancia o serviço e chama o método
            var faceService = new FaceRecognitionService();
            await faceService.CriarGrupoSeNaoExistirAsync();

            await Task.Delay(2000); // Aguarda um pouco para exibir a logo

            if (Application.Current?.Windows.Count > 0)
            {
                Application.Current.Windows[0].Page = new AppShell();
            }
        }
        catch (Exception ex)
        {
            // --- INÍCIO DO CÓDIGO DE DEPURAÇÃO (Corrigido para Face++) ---

            // Vamos ler as chaves do Face++ que a app *acha* que tem
            string debugKey = SAAD.Helpers.SecretsManager.FaceApiKey;
            string debugSecret = SAAD.Helpers.SecretsManager.FaceApiSecret;

            // Mostrar pré-visualização das chaves
            string keyPreview = string.IsNullOrEmpty(debugKey) ? "VAZIA" : $"{debugKey.Substring(0, 4)}...{debugKey.Substring(debugKey.Length - 4)}";
            string secretPreview = string.IsNullOrEmpty(debugSecret) ? "VAZIA" : $"{debugSecret.Substring(0, 4)}...{debugSecret.Substring(debugSecret.Length - 4)}";

            string debugMessage = $"Face++ ApiKey: {keyPreview}\n" +
                                  $"Face++ ApiSecret: {secretPreview}\n\n" +
                                  $"Erro: {ex.Message}";

            bool tentarNovamente = await DisplayAlert("Erro de Conexão (Debug Face++)",
                                        debugMessage,
                                        "Tentar Novamente", "Sair");
            // --- FIM DO CÓDIGO DE DEPURAÇÃO ---

            if (tentarNovamente)
            {
                OnAppearing();
            }
            else
            {
                Application.Current.Quit();
            }
        
        }
    }
}