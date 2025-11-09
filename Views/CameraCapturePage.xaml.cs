using Camera.MAUI;
using CommunityToolkit.Mvvm.Messaging;
using Firebase.Database;
using SAAD.Helpers;
using SAAD.Messages;
using SAAD.Models;
using SAAD.Services;

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

            // CORREÇÃO AQUI: Não passamos mais argumentos, ele pega sozinho do SecretsManager
            faceService = new FaceRecognitionService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
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
            cameraView.Camera = cameraView.Cameras.FirstOrDefault(c => c.Position == CameraPosition.Front);
        }

        private async void CaptureButton_Clicked(object sender, EventArgs e)
        {
            SetLoading(true);
            var photoStream = await cameraView.TakePhotoAsync();
            if (photoStream == null)
            {
                SetLoading(false);
                return;
            }
            await StartRecognitionProcess(photoStream);
        }

        private async Task StartRecognitionProcess(Stream photoStream)
        {
            try
            {
                var alunos = await firebaseClient.Child("users").OnceAsync<User>();
                var listaDeAlunos = alunos
                    .Where(u => u.Object.UserType == "Aluno")
                    .Select(u => u.Object)
                    .ToList();

                var alunoReconhecido = await faceService.ReconhecerAluno(photoStream, listaDeAlunos);
                SetLoading(false);

                if (alunoReconhecido != null)
                {
                    WeakReferenceMessenger.Default.Send(new AlunoReconhecidoMessage(alunoReconhecido, isEntrada: true));
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Não reconhecido", "Nenhum aluno foi identificado.", "Tentar Novamente");
                }
            }
            catch (Exception ex)
            {
                SetLoading(false);
                await DisplayAlert("Erro", $"Falha: {ex.Message}", "OK");
            }
            finally
            {
                if (photoStream != null) await photoStream.DisposeAsync();
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