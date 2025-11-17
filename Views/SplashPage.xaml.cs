using SAAD.Services;
using System.Threading.Tasks;

namespace SAAD.Views;

public partial class SplashPage : ContentPage
{
    //  Crie uma variável privada para guardar o serviço 
    private readonly FaceRecognitionService _faceService;

    //  MODIFIQUE O CONSTRUTOR AQUI 
    // Em vez de "public SplashPage()", agora ele "pede" o serviço
    public SplashPage(FaceRecognitionService faceRecognitionService)
    {
        InitializeComponent();
        // Salva o serviço que foi "injetado" na sua variável
        _faceService = faceRecognitionService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            //  USE O SERVIÇO DA SUA VARIÁVEL 
            // (Não precisa mais do 'new FaceRecognitionService()')
            await _faceService.CriarGrupoSeNaoExistirAsync();

            await Task.Delay(2000); // Aguarda um pouco para exibir a logo

            if (Application.Current?.Windows.Count > 0)
            {
                Application.Current.Windows[0].Page = new AppShell();
            }
        }
        catch (Exception ex)
        {
            // --- O seu excelente código de depuração continua o mesmo ---
            string debugKey = SAAD.Helpers.SecretsManager.FaceApiKey;
            string debugSecret = SAAD.Helpers.SecretsManager.FaceApiSecret;

            string keyPreview = string.IsNullOrEmpty(debugKey) ? "VAZIA" : $"{debugKey.Substring(0, 4)}...{debugKey.Substring(debugKey.Length - 4)}";
            string secretPreview = string.IsNullOrEmpty(debugSecret) ? "VAZIA" : $"{debugSecret.Substring(0, 4)}...{debugSecret.Substring(debugSecret.Length - 4)}";

            string debugMessage = $"Face++ ApiKey: {keyPreview}\n" +
                                    $"Face++ ApiSecret: {secretPreview}\n\n" +
                                    $"Erro: {ex.Message}";

            bool tentarNovamente = await DisplayAlert("Erro de Conexão (Debug Face++)",
                                                        debugMessage,
                                                        "Tentar Novamente", "Sair");

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