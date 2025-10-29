using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using SAAD.Models;
using SAAD.Services;
using SAAD.Helpers;
using SAAD.Messages;
using Firebase.Database;
using Firebase.Database.Query;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
<<<<<<< HEAD
=======
using SAAD.Services;

>>>>>>> e7a75db0abf4951fa25ff787f02f74fa389a8764

namespace SAAD.Views
{
    public partial class CameraCapturePage : ContentPage
    {
        private FirebaseClient firebaseClient;
        private FaceRecognitionService faceService;

        public CameraCapturePage()
        {
            InitializeComponent();

            firebaseClient = new FirebaseClient(
                SecretsManager.FirebaseUrl,
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(SecretsManager.FirebaseSecret)
                });

            faceService = new FaceRecognitionService(SecretsManager.AzureEndpoint, SecretsManager.AzureKey);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
<<<<<<< HEAD
            await CapturarEReconhecer();
        }

        private async Task CapturarEReconhecer()
=======

            // Aguarda a renderização da interface
            await Task.Delay(500);

            StartCameraDetection();
        }


        private async void StartCameraDetection()
>>>>>>> e7a75db0abf4951fa25ff787f02f74fa389a8764
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo == null)
                {
<<<<<<< HEAD
                    await DisplayAlert("Erro", "Nenhuma foto foi capturada.", "OK");
                    await Navigation.PopAsync();
                    return;
                }
                else {
                    // var photo = await MediaPicker.CapturePhotoAsync();
                    // if (photo == null) continue;

                    using var stream = await photo.OpenReadAsync();

                    // Exibe imagem capturada no preview
                    CameraPreview.Source = ImageSource.FromStream(() => stream);

                    var imagemCadastrada = await FileSystem.OpenAppPackageFileAsync("imagem_cadastrada.jpg");
                    var rostoDetectado = await AzureFaceService.CompararAsync(stream, imagemCadastrada);
                }
                    if (rostoDetectado)
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
>>>>>>> e7a75db0abf4951fa25ff787f02f74fa389a8764
                }

                using var streamFoto = await photo.OpenReadAsync();

                var alunos = await firebaseClient.Child("users").OnceAsync<User>();
                var listaDeAlunos = alunos
                    .Where(u => u.Object.UserType == "Aluno")
                    .Select(u => u.Object)
                    .ToList();

                var alunoReconhecido = await faceService.ReconhecerAluno(streamFoto, listaDeAlunos);

                if (alunoReconhecido != null)
                {
                    // Por padrão, assume entrada. Você pode alterar isso conforme o botão que chamou a página.
                    WeakReferenceMessenger.Default.Send(new AlunoReconhecidoMessage(alunoReconhecido, isEntrada: true));
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Não reconhecido", "Nenhum aluno foi identificado.", "OK");
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha na captura ou reconhecimento: {ex.Message}", "OK");
                await Navigation.PopAsync();
            }
        }
<<<<<<< HEAD
=======

        private async Task EnviarParaAzure(Stream imagemCapturada)
        {
            var imagemCadastrada = await FileSystem.OpenAppPackageFileAsync("imagem_cadastrada.jpg");

            var resultado = await AzureFaceService.CompararAsync(imagemCapturada, imagemCadastrada);

            if (resultado)
                await DisplayAlert("Verificação", "Rosto compatível com cadastro!", "OK");
            else
                await DisplayAlert("Verificação", "Rosto não reconhecido.", "OK");
        }
>>>>>>> e7a75db0abf4951fa25ff787f02f74fa389a8764
    }
}
