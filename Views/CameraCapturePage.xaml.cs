using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using SAAD.Models;
using SAAD.Services; // Removido using duplicado
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

        // --- CORREÇÃO: Método 'CapturarEReconhecer' não tinha corpo {} ---
        private async Task CapturarEReconhecer()
        {
            // Aguarda a renderização da interface
            await Task.Delay(500);

            // Inicia a detecção (método foi renomeado e corrigido)
            await StartRecognitionProcess();
        }

        // --- CORREÇÃO: Lógica unificada e estruturada corretamente ---
        private async Task StartRecognitionProcess()
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

                // Lê a foto para um array de bytes. Isso permite usar o stream
                // para a API e para o preview sem erros de "stream consumido".
                byte[] photoBytes;
                using (var stream = await photo.OpenReadAsync())
                {
                    using (var ms = new MemoryStream())
                    {
                        await stream.CopyToAsync(ms);
                        photoBytes = ms.ToArray();
                    }
                }

                // Exibe imagem capturada no preview
                CameraPreview.Source = ImageSource.FromStream(() => new MemoryStream(photoBytes));

                // Mostra o indicador de carregamento
                InstructionLabel.Text = "Analisando...";
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                // Cria um novo stream para a API de reconhecimento
                using (var apiStream = new MemoryStream(photoBytes))
                {
                    // Pega a lista de alunos do Firebase
                    var alunos = await firebaseClient.Child("users").OnceAsync<User>();
                    var listaDeAlunos = alunos
                        .Where(u => u.Object.UserType == "Aluno")
                        .Select(u => u.Object)
                        .ToList();

                    // Reconhece o aluno
                    var alunoReconhecido = await faceService.ReconhecerAluno(apiStream, listaDeAlunos);

                    // Esconde o indicador de carregamento
                    LoadingIndicator.IsVisible = false;
                    LoadingIndicator.IsRunning = false;

                    if (alunoReconhecido != null)
                    {
                        // Envia a mensagem para a página anterior
                        WeakReferenceMessenger.Default.Send(new AlunoReconhecidoMessage(alunoReconhecido, isEntrada: true));
                        await DisplayAlert("Sucesso", $"Aluno '{alunoReconhecido.Nome}' reconhecido!", "OK");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Não reconhecido", "Nenhum aluno foi identificado.", "OK");
                        await Navigation.PopAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // --- CORREÇÃO: O bloco 'catch' estava fora do 'try' ---
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                await DisplayAlert("Erro", $"Falha na captura ou reconhecimento: {ex.Message}", "OK");
                await Navigation.PopAsync();
            }
        }

        // --- REMOVIDO: O método 'EnviarParaAzure' era parte da lógica
        // conflitante e não estava sendo usado pelo fluxo principal.
    }
}