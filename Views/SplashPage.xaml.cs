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
            bool tentarNovamente = await DisplayAlert("Erro de Conexão",
                $"Não foi possível conectar ao serviço de reconhecimento facial.\nErro: {ex.Message}",
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