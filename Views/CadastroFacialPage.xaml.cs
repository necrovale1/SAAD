using Firebase.Database;
using Firebase.Database.Query;
using SAAD2.Models;
using System.Text;
using SAAD2.Helpers;

namespace SAAD2.Views
{
    public partial class CadastroFacialPage : ContentPage
    {
        private readonly FirebaseClient firebaseClient;
        private string base64Image;

        public CadastroFacialPage()
        {
            InitializeComponent();
            var firebaseClient = new FirebaseClient(
            SecretsManager.FirebaseUrl,
            new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(SecretsManager.FirebaseSecret)
            });
        }

        private async void OnCapturePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo == null) return;

                using var stream = await photo.OpenReadAsync();
                base64Image = ConvertImageToBase64(stream);

                PhotoImage.Source = ImageSource.FromStream(() => stream);
                PhotoImage.IsVisible = true;
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                await DisplayAlert("Foto Capturada", "A imagem foi capturada com sucesso!", "OK");
                SaveFaceButton.IsVisible = true;

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private async void OnSaveFaceClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RaEntry.Text))
            {
                await DisplayAlert("Erro", "Digite o RA antes de salvar.", "OK");
                return;
            }

            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                var aluno = await FindStudentByRA(RaEntry.Text);
                if (aluno == null)
                {
                    await DisplayAlert("Erro", "RA não encontrado.", "OK");
                    return;
                }

                aluno.FaceImageBase64 = base64Image;
                await firebaseClient.Child("users").Child(aluno.Uid).PutAsync(aluno);

                await DisplayAlert("Sucesso", "Imagem facial salva com sucesso!", "OK");
                RaEntry.Text = string.Empty;
                PhotoImage.IsVisible = false;
                SaveFaceButton.IsVisible = true; 

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private string ConvertImageToBase64(Stream imageStream)
        {
            using var memoryStream = new MemoryStream();
            imageStream.CopyTo(memoryStream);
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        private async Task<User> FindStudentByRA(string ra)
        {
            var users = await firebaseClient.Child("users").OnceAsync<User>();
            return users
                .Where(u => u.Object.RegistroAcademico == ra && u.Object.UserType == "Aluno")
                .Select(u => {
                    u.Object.Uid = u.Key;
                    return u.Object;
                })
                .FirstOrDefault();
        }
    }
}
