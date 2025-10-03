using Camera.MAUI;

namespace SAAD2.Views;

// A palavra "partial" é essencial para conectar este código ao XAML
public partial class FaceAuthPage : ContentPage
{
    public FaceAuthPage()
    {
        // Este comando conecta os controles do XAML (como o cameraView) a este arquivo
        InitializeComponent();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Garante que a câmera seja desligada ao sair da página
        if (cameraView.IsCameraStarted)
        {
            cameraView.StopCameraAsync();
        }
    }

    private void CameraView_CamerasLoaded(object sender, EventArgs e)
    {
        // Verifica se existem câmeras no dispositivo
        if (cameraView.Cameras.Count > 0)
        {
            // Seleciona a câmera frontal como padrão
            cameraView.Camera = cameraView.Cameras.FirstOrDefault(c => c.Position == CameraPosition.Front);

            // Inicia a visualização da câmera na thread principal
            MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await cameraView.StartCameraAsync();
            });
        }
    }
}