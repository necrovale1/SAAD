using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using FaceRecognition; 

namespace SAAD.Views
{
    public partial class CameraCapturePage : ContentPage
    {
        private int tentativas = 0;

        public CameraCapturePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            StartCameraDetection();
        }

        private async void StartCameraDetection()
        {
            InstructionLabel.Text = "Aproxime o rosto";

            while (tentativas < 10)
            {
                tentativas++;

                try
                {
                    var photo = await MediaPicker.CapturePhotoAsync();
                    if (photo == null) continue;

                    using var stream = await photo.OpenReadAsync();

                    // Exibe imagem capturada no preview
                    CameraPreview.Source = ImageSource.FromStream(() => stream);

                    // Detecção facial real com FaceRecognition
                    var result = await FaceRecognitionService.DetectAsync(stream);

                    if (result.FaceDetected)
                    {
                        InstructionLabel.Text = "Rosto detectado!";
                        LoadingIndicator.IsVisible = true;
                        LoadingIndicator.IsRunning = true;

                        await Task.Delay(1000); // Simula tempo de captura

                        await DisplayAlert("Sucesso", "Imagem capturada com sucesso!", "OK");

                        // Enviar para Azure Face API
                        await EnviarParaAzure(stream);

                        LoadingIndicator.IsVisible = false;
                        LoadingIndicator.IsRunning = false;
                        break;
                    }
                    else
                    {
                        InstructionLabel.Text = "Rosto não detectado. Tente novamente sem óculos ou boné.";
                        await Task.Delay(1500);
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro", ex.Message, "OK");
                }
            }

            if (tentativas >= 10)
            {
                await DisplayAlert("Falha", "Não foi possível detectar o rosto após várias tentativas.", "OK");
            }
        }

        private async Task EnviarParaAzure(Stream imageStream)
        {
      
            var resultado = await AzureFaceService.CompararAsync(imageStream, imagemCadastrada);
        }
    }
}
