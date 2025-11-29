using Camera.MAUI;
using CommunityToolkit.Mvvm.Messaging;
using Firebase.Database;
using Firebase.Database.Query;
using SAAD.Helpers;
using SAAD.Messages;
using SAAD.Models;
using SAAD.Services;

namespace SAAD.Views
{
    [QueryProperty(nameof(IsEntrada), "isEntrada")]
    public partial class CameraCapturePage : ContentPage
    {
        private readonly FaceRecognitionService _faceService;
        private FirebaseClient firebaseClient;
        private FaceRecognitionService faceService;
        public bool IsEntrada { get; set; }
        public CameraCapturePage()
        {
            InitializeComponent();
            _faceService = faceService;

            firebaseClient = new FirebaseClient(
                SecretsManager.FirebaseUrl,
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(SecretsManager.FirebaseSecret)
                });

            // Construtor do serviço (agora não precisa de argumentos)
            faceService = new FaceRecognitionService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.Title = IsEntrada ? "Registrar Entrada" : "Registrar Saída"; // define o título com base na query

            // 1. Verificar Permissões
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Erro", "É necessária permissão da câmara.", "OK");
                    await Navigation.PopAsync();
                    return;
                }
            }

            // NÃO tente definir a câmara aqui. O componente CameraView ainda pode estar a carregar.
            // Apenas aguarde o evento CamerasLoaded disparar.
        }

        private async void CameraView_CamerasLoaded(object sender, EventArgs e)
        {
            // Este método é chamado automaticamente quando o plugin termina de listar as câmaras
            if (cameraView.Cameras.Count > 0)
            {
                // Tenta pegar a frontal, senão pega a primeira disponível
                cameraView.Camera = cameraView.Cameras.FirstOrDefault(c => c.Position == CameraPosition.Front)
                                    ?? cameraView.Cameras.First();

                // Inicia a câmara explicitamente
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await cameraView.StopCameraAsync(); // Garante que não há nada pendente
                    await cameraView.StartCameraAsync();
                });
            }
            else
            {
                await DisplayAlert("Erro", "Nenhuma câmara detetada no dispositivo.", "OK");
            }
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

        // --- MÉTODO CORRIGIDO (Início) ---

        /// <summary>
        /// Este é o método que foi corrigido para usar a nova lógica do Face++
        /// </summary>
        private async Task StartRecognitionProcess(Stream photoStream)
        {
            try
            {
                // 1. Chamar o serviço Face++ (que agora só precisa da foto)
                // O nome do método mudou de ReconhecerAluno para ReconhecerRostoAsync
                string alunoIdReconhecido = await faceService.ReconhecerRostoAsync(photoStream);

                if (!string.IsNullOrEmpty(alunoIdReconhecido))
                {
                    // 2. Se o Face++ encontrou, ele retorna o ID do Firebase.
                    // Agora, vamos buscar *apenas* esse aluno no Firebase.
                    var aluno = await firebaseClient
                                    .Child("users")
                                    .Child(alunoIdReconhecido) // Busca o aluno pelo ID (Key)
                                    .OnceSingleAsync<User>(); 

                    if (aluno != null)
                    {
                        // 3. Sucesso! Encontramos o aluno.
                        SetLoading(false); 
                        WeakReferenceMessenger.Default.Send(new AlunoReconhecidoMessage(aluno, isEntrada: true));
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        // Isso seria estranho - O Face++ encontrou um ID que não existe no Firebase
                        SetLoading(false);
                        await DisplayAlert("Erro de Sincronia", "O rosto foi reconhecido, mas o aluno não foi encontrado na base de dados.", "OK");
                    }
                }
                else
                {
                    // 4. Face++ não encontrou ninguém
                    SetLoading(false);
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
        
        // --- MÉTODO CORRIGIDO (Fim) ---

        private void SetLoading(bool isLoading)
        {
            LoadingIndicator.IsVisible = isLoading;
            LoadingIndicator.IsRunning = isLoading;
            CaptureButton.IsVisible = !isLoading;
            InstructionLabel.Text = isLoading ? "Analisando..." : "Posicione seu rosto dentro da elipse";
        }
    }
}