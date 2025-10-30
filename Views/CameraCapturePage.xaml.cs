using Camera.MAUI; // Adicione este
using CommunityToolkit.Mvvm.Messaging;
using Firebase.Database;
using Firebase.Database.Query;
using SAAD.Helpers;
using SAAD.Messages;
using SAAD.Models;
using SAAD.Services;
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

            // Verifica a permissão da câmera
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Erro", "A permissão da câmera é necessária.", "OK");
                    await Navigation.PopAsync();
                    return;
                }
            }

            // Inicia a câmera
            cameraView.Camera = cameraView.Cameras.FirstOrDefault(c => c.Position == CameraPosition.Front);
        }

        private async void CaptureButton_Clicked(object sender, EventArgs e)
        {
            SetLoading(true);

            // Tira a foto usando o Camera.MAUI
            var photoStream = await cameraView.TakePhotoAsync();

            if (photoStream == null)
            {
                await DisplayAlert("Erro", "Não foi possível capturar a foto.", "OK");
                SetLoading(false);
                return;
            }

            // Agora, usamos a MESMA lógica que você já tinha
            await StartRecognitionProcess(photoStream);
        }

        private async Task StartRecognitionProcess(Stream photoStream)
        {
            try
            {
                // Pega a lista de alunos do Firebase
                var alunos = await firebaseClient.Child("users").OnceAsync<User>();
                var listaDeAlunos = alunos
                    .Where(u => u.Object.UserType == "Aluno")
                    .Select(u => u.Object)
                    .ToList();

                // Reconhece o aluno usando seu serviço
                // O 'FaceRecognitionService' já está pronto para isso (da nossa última conversa)
                var alunoReconhecido = await faceService.ReconhecerAluno(photoStream, listaDeAlunos);

                SetLoading(false);

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
                    // Fica na página para tentar de novo
                }
            }
            catch (Exception ex)
            {
                SetLoading(false);
                await DisplayAlert("Erro", $"Falha no reconhecimento: {ex.Message}", "OK");
            }
            finally
            {
                // Limpa o stream da foto
                if (photoStream != null)
                {
                    await photoStream.DisposeAsync();
                }
            }
        }

        private void SetLoading(bool isLoading)
        {
            LoadingIndicator.IsVisible = isLoading;
            LoadingIndicator.IsRunning = isLoading;
            CaptureButton.IsVisible = !isLoading;
            InstructionLabel.Text = isLoading ? "Analisando..." : "Posicione seu rosto dentro da elipse";
        }
    }
}