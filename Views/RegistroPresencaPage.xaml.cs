using Firebase.Database;
using Firebase.Database.Query;
using SAAD.Helpers;
using SAAD.Models;
using SAAD.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using UserModel = SAAD.Models.User;

namespace SAAD.Views
{
    public partial class RegistroPresencaPage : ContentPage
    {
        private FirebaseClient firebaseClient;
        private readonly FaceDetectionService _faceDetectionService;

        public RegistroPresencaPage()
        {
            InitializeComponent();

            var url = SecretsManager.FirebaseUrl;
            var secret = SecretsManager.FirebaseSecret;

            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(secret))
            {
                DisplayAlert("Erro", "Credenciais do Firebase não foram carregadas corretamente.", "OK");
                return;
            }

            firebaseClient = new FirebaseClient(url, new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(secret)
            });

            _faceDetectionService = new FaceDetectionService();
        }

        private async void OnRegistrarPresencaClicked(object sender, EventArgs e)
        {
            SetUIState(true);

            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo == null)
                {
                    await DisplayAlert("Erro", "Não foi possível acessar a câmera.", "OK");
                    return;
                }

                using var stream = await photo.OpenReadAsync();
                var base64Atual = ConvertImageToBase64(stream);
                PhotoImage.Source = ImageSource.FromStream(() => stream);
                PhotoImage.IsVisible = true;

                var alunos = await firebaseClient.Child("users").OnceAsync<UserModel>();
                var alunoReconhecido = alunos
                    .Where(u => u.Object.UserType == "Aluno" && !string.IsNullOrEmpty(u.Object.FaceImageBase64))
                    .Select(u =>
                    {
                        u.Object.Uid = u.Key;
                        return u.Object;
                    })
                    .FirstOrDefault(u => _faceDetectionService.CompararImagensAsync(u.FaceImageBase64, base64Atual).Result);

                if (alunoReconhecido != null)
                {
                    await DisplayAlert("Autenticação Facial", $"Rosto reconhecido: {alunoReconhecido.Nome}", "OK");
                    await HandlePresenceRegistration(alunoReconhecido, isEntrada: true);
                }
                else
                {
                    await DisplayAlert("Rosto não reconhecido",
                        "Tente novamente sem óculos, boné ou em ambiente iluminado.",
                        "OK");

                    TryAgainButton.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
            finally
            {
                SetUIState(false);
            }
        }

        private void OnTryAgainClicked(object sender, EventArgs e)
        {
            TryAgainButton.IsVisible = false;
            PhotoImage.IsVisible = false;
            OnRegistrarPresencaClicked(sender, e);
        }

        private async Task HandlePresenceRegistration(UserModel aluno, bool isEntrada)
        {
            var aulaAtual = await GetCurrentClass();
            if (aulaAtual == null)
            {
                await DisplayAlert("Aviso", "Não há nenhuma aula programada para o horário atual.", "OK");
                return;
            }

            if (isEntrada)
            {
                await RegisterEntrada(aluno, aulaAtual);
            }
            else
            {
                await RegisterSaida(aluno);
            }
        }

        private async Task<Horario> GetCurrentClass()
        {
            var horarios = await firebaseClient.Child("horarios").OnceAsync<Horario>();
            var today = DateTime.Now.DayOfWeek;
            var now = DateTime.Now.TimeOfDay;

            return horarios
                .Select(h => h.Object)
                .FirstOrDefault(h => h.DiaDaSemana == today && h.HoraInicio <= now && h.HoraFim >= now);
        }

        private async Task RegisterEntrada(UserModel aluno, Horario aula)
        {
            var presencasHoje = await firebaseClient.Child("presencas").OnceAsync<Presenca>();
            var entradaAberta = presencasHoje
                .FirstOrDefault(p => p.Object.StudentUid == aluno.Uid && p.Object.Data.Date == DateTime.Today && p.Object.HoraSaida == null);

            if (entradaAberta != null)
            {
                await DisplayAlert("Aviso", $"{aluno.Nome}, sua entrada já foi registrada às {entradaAberta.Object.HoraEntrada:HH:mm}.", "OK");
                return;
            }

            var novaPresenca = new Presenca
            {
                StudentUid = aluno.Uid,
                StudentName = aluno.Nome,
                HorarioKey = aula.MateriaKey,
                MateriaName = aula.MateriaName,
                Data = DateTime.Today,
                HoraEntrada = DateTime.Now
            };

            await firebaseClient.Child("presencas").PostAsync(novaPresenca);
            await DisplayAlert("Sucesso!", $"Entrada de {aluno.Nome} registrada às {novaPresenca.HoraEntrada:HH:mm} para {aula.MateriaName}.", "OK");
        }

        private async Task RegisterSaida(UserModel aluno)
        {
            var presencas = await firebaseClient.Child("presencas").OnceAsync<Presenca>();
            var entradaAberta = presencas
                .FirstOrDefault(p => p.Object.StudentUid == aluno.Uid && p.Object.Data.Date == DateTime.Today && p.Object.HoraSaida == null);

            if (entradaAberta == null)
            {
                await DisplayAlert("Erro", "Nenhum registro de entrada aberto encontrado para hoje.", "OK");
                return;
            }

            entradaAberta.Object.HoraSaida = DateTime.Now;
            await firebaseClient.Child("presencas").Child(entradaAberta.Key).PutAsync(entradaAberta.Object);
            await DisplayAlert("Até logo!", $"Saída registrada às {entradaAberta.Object.HoraSaida:HH:mm}.", "OK");
        }

        private string ConvertImageToBase64(Stream imageStream)
        {
            using var memoryStream = new MemoryStream();
            imageStream.CopyTo(memoryStream);
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        private void SetUIState(bool isLoading)
        {
            LoadingIndicator.IsVisible = isLoading;
            LoadingIndicator.IsRunning = isLoading;
            RegistrarPresencaButton.IsEnabled = !isLoading;
            TryAgainButton.IsEnabled = !isLoading;
        }
    }
}
