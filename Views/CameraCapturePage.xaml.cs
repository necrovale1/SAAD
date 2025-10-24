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
            await CapturarEReconhecer();
        }

        private async Task CapturarEReconhecer()
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo == null)
                {
                    await DisplayAlert("Erro", "Nenhuma foto foi capturada.", "OK");
                    await Navigation.PopAsync();
                    return;
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
    }
}
