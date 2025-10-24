using CommunityToolkit.Mvvm.Messaging;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using SAAD.Helpers;
using SAAD.Messages;
using SAAD.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SAAD.Views
{
    public partial class CadastroAlunoPage : ContentPage
    {
        private FirebaseClient firebaseClient;
        private string fotoBase64;

        public CadastroAlunoPage()
        {
            InitializeComponent();

            firebaseClient = new FirebaseClient(
                SecretsManager.FirebaseUrl,
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(SecretsManager.FirebaseSecret)
                });
        }

        private async void OnCapturarFotoClicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo == null) return;

                using var stream = await photo.OpenReadAsync();
                FotoPreview.Source = ImageSource.FromStream(() => stream);
                FotoPreview.IsVisible = true;

                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                fotoBase64 = Convert.ToBase64String(memoryStream.ToArray());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao capturar foto: {ex.Message}", "OK");
            }
        }

        private async void OnSalvarAlunoClicked(object sender, EventArgs e)
        {
            var nome = NomeEntry.Text?.Trim();
            var email = EmailEntry.Text?.Trim();

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fotoBase64))
            {
                await DisplayAlert("Aviso", "Preencha todos os campos e capture uma foto.", "OK");
                return;
            }

            var novoAluno = new User
            {
                Nome = nome,
                Email = email,
                UserType = "Aluno",
                FaceImageBase64 = fotoBase64
            };

            await firebaseClient.Child("users").PostAsync(novoAluno);
            await DisplayAlert("Sucesso", "Aluno cadastrado com sucesso!", "OK");

            NomeEntry.Text = "";
            EmailEntry.Text = "";
            FotoPreview.IsVisible = false;
            fotoBase64 = null;
            WeakReferenceMessenger.Default.Send(new AlunoReconhecidoMessage(novoAluno, isEntrada: false));
        }
    }
}
