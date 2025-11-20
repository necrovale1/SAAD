using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Maui.Media;
using SAAD.Helpers;
using SAAD.Models;
using SAAD.Services;
using System.Text;

namespace SAAD.Views
{
    public partial class CadastroAlunoPage : ContentPage
    {
        private readonly FirebaseClient firebaseClient;
        private readonly FaceRecognitionService _faceService; // Serviço do Face++
        private string fotoBase64;
        private byte[] fotoBytes; // Guardamos os bytes para enviar ao serviço

        public CadastroAlunoPage()
        {
            InitializeComponent();

            firebaseClient = new FirebaseClient(
                SecretsManager.FirebaseUrl,
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(SecretsManager.FirebaseSecret)
                });

            _faceService = new FaceRecognitionService();
        }

        private async void OnCapturarFotoClicked(object sender, EventArgs e)
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    var photo = await MediaPicker.CapturePhotoAsync();
                    if (photo == null) return;

                    using var stream = await photo.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);

                    fotoBytes = memoryStream.ToArray();
                    fotoBase64 = Convert.ToBase64String(fotoBytes);

                    FotoPreview.Source = ImageSource.FromStream(() => new MemoryStream(fotoBytes));
                    FotoPreview.IsVisible = true;
                }
                else
                {
                    await DisplayAlert("Erro", "Câmera não suportada.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha na câmera: {ex.Message}", "OK");
            }
        }

        private async void OnSalvarAlunoClicked(object sender, EventArgs e)
        {
            if (IsBusy) return;

            var nome = NomeEntry.Text?.Trim();
            var email = EmailEntry.Text?.Trim();
            var ra = RaEntry.Text?.Trim();

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fotoBase64))
            {
                await DisplayAlert("Aviso", "Preencha todos os campos e tire a foto.", "OK");
                return;
            }

            IsBusy = true;
            LoadingIndicator.IsRunning = true;
            LoadingIndicator.IsVisible = true;
            BtnSalvar.IsEnabled = false;

            try
            {
                var novoAluno = new User
                {
                    Nome = nome,
                    Email = email,
                    RegistroAcademico = ra ?? "",
                    UserType = "Aluno",
                    FaceImageBase64 = fotoBase64
                };

                // 1. Salva no Firebase
                var result = await firebaseClient.Child("users").PostAsync(novoAluno);
                var firebaseId = result.Key;

                // 2. Treina no Face++ (Passo crucial!)
                // Usamos um novo stream a partir dos bytes guardados
                using (var faceStream = new MemoryStream(fotoBytes))
                {
                    bool sucesso = await _faceService.AdicionarRostoAoFaceSetAsync(faceStream, firebaseId);

                    if (sucesso)
                    {
                        await DisplayAlert("Sucesso", "Aluno cadastrado e rosto treinado!", "OK");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Aviso", "Aluno salvo no banco, mas o rosto não foi detetado pelo Face++. Tente novamente com melhor iluminação.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao salvar: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                BtnSalvar.IsEnabled = true;
            }
        }
    }
}