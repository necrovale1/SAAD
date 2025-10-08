using Firebase.Database;
using Firebase.Database.Query;
using SAAD2.Enums;
using SAAD2.Helpers;
using SAAD2.Models;
using SAAD2.Services; // Importa o nosso novo servi�o de dete��o facial
using System.Linq;
using UserModel = SAAD2.Models.User;

namespace SAAD2.Views
{
    public partial class RegistroPresencaPage : ContentPage
    {
        private readonly FirebaseClient firebaseClient;
        private readonly FaceDetectionService _faceDetectionService;

        public RegistroPresencaPage()
        {
            InitializeComponent();
            firebaseClient = new FirebaseClient("https://saad-1fd38-default-rtdb.firebaseio.com/");
            _faceDetectionService = new FaceDetectionService();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateThemeIcon();
        }

        // --- L�GICA DO RECONHECIMENTO FACIAL ---
        private async void OnFacialRecognitionClicked(object sender, EventArgs e)
        {
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await DisplayAlert("Erro", "Nenhuma c�mara dispon�vel neste dispositivo.", "OK");
                    return;
                }

                FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo == null) return; // Utilizador cancelou a captura

                // Exibe a foto tirada
                using var stream = await photo.OpenReadAsync();
                var photoBytes = ReadStream(stream);
                PhotoImage.Source = ImageSource.FromStream(() => new MemoryStream(photoBytes));
                PhotoImage.IsVisible = true;

                SetUIState(true); // Bloqueia a UI enquanto processa

                // Usa o nosso servi�o de dete��o de rosto
                using var detectionStream = new MemoryStream(photoBytes);
                var (hasFace, message) = await _faceDetectionService.DetectFaceAsync(detectionStream);

                await DisplayAlert("Resultado da Dete��o", message, "OK");

                if (hasFace)
                {
                    // ---- PR�XIMO PASSO L�GICO ----
                    // Se encontr�mos um rosto, aqui chamar�amos o servi�o de RECONHECIMENTO
                    // para saber QUEM � a pessoa. Por agora, apenas confirmamos a dete��o.
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro Inesperado", $"Ocorreu um erro: {ex.Message}", "OK");
            }
            finally
            {
                SetUIState(false); // Desbloqueia a UI
            }
        }

        // --- L�GICA DO REGISTO POR RA ---
        private async void OnEntradaClicked(object sender, EventArgs e)
        {
            await HandlePresenceRegistration(isEntrada: true);
        }

        private async void OnSaidaClicked(object sender, EventArgs e)
        {
            await HandlePresenceRegistration(isEntrada: false);
        }

        private async Task HandlePresenceRegistration(bool isEntrada)
        {
            if (string.IsNullOrWhiteSpace(RaEntry.Text))
            {
                await DisplayAlert("Erro", "Por favor, digite o seu RA.", "OK");
                return;
            }

            SetUIState(isLoading: true);

            try
            {
                var aluno = await FindStudentByRA(RaEntry.Text);
                if (aluno == null)
                {
                    await DisplayAlert("Erro", "RA n�o encontrado. Verifique o n�mero digitado.", "OK");
                    return;
                }

                var aulaAtual = await GetCurrentClass();
                if (aulaAtual == null)
                {
                    await DisplayAlert("Aviso", "N�o h� nenhuma aula programada para o hor�rio atual.", "OK");
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
            catch (Exception ex)
            {
                await DisplayAlert("Erro Inesperado", $"Ocorreu um erro: {ex.Message}", "OK");
            }
            finally
            {
                SetUIState(isLoading: false);
                RaEntry.Text = string.Empty;
            }
        }

        // --- M�TODOS AUXILIARES ---
        private async Task<UserModel> FindStudentByRA(string ra)
        {
            var users = await firebaseClient.Child("users").OnceAsync<UserModel>();
            return users
                .Where(u => u.Object.RegistroAcademico == ra && u.Object.UserType == "Aluno")
                .Select(u => {
                    u.Object.Uid = u.Key;
                    return u.Object;
                })
                .FirstOrDefault();
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
                await DisplayAlert("Aviso", $"{aluno.Nome}, a sua entrada j� foi registrada �s {entradaAberta.Object.HoraEntrada:HH:mm}.", "OK");
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
            await DisplayAlert("Sucesso!", $"Entrada de {aluno.Nome} registrada com sucesso �s {novaPresenca.HoraEntrada:HH:mm} para a aula de {aula.MateriaName}.", "OK");
        }

        private async Task RegisterSaida(UserModel aluno)
        {
            var presencas = await firebaseClient.Child("presencas").OnceAsync<Presenca>();
            var entradaAberta = presencas
                .FirstOrDefault(p => p.Object.StudentUid == aluno.Uid && p.Object.Data.Date == DateTime.Today && p.Object.HoraSaida == null);

            if (entradaAberta == null)
            {
                await DisplayAlert("Erro", "Nenhum registro de entrada aberto foi encontrado para voc� hoje. Por favor, registre sua entrada primeiro.", "OK");
                return;
            }

            entradaAberta.Object.HoraSaida = DateTime.Now;
            await firebaseClient.Child("presencas").Child(entradaAberta.Key).PutAsync(entradaAberta.Object);
            await DisplayAlert("At� Logo!", $"Sa�da de {aluno.Nome} registrada com sucesso �s {entradaAberta.Object.HoraSaida:HH:mm}.", "OK");
        }

        private void SetUIState(bool isLoading)
        {
            LoadingIndicator.IsVisible = isLoading;
            LoadingIndicator.IsRunning = isLoading;
            EntradaButton.IsEnabled = !isLoading;
            SaidaButton.IsEnabled = !isLoading;
            FacialButton.IsEnabled = !isLoading; // Controla o novo bot�o tamb�m
        }

        private byte[] ReadStream(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            stream.Position = 0;
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        // --- L�GICA DO TEMA ---
        private void OnThemeIconTapped(object sender, TappedEventArgs e)
        {
            var app = (App)Application.Current;
            app.ToggleTheme();
            UpdateThemeIcon();
        }

        private void UpdateThemeIcon()
        {
            var app = (App)Application.Current;
            ThemeIconLabel.Text = app.CurrentTheme == Theme.Light
                ? FontAwesomeIcons.Sun
                : FontAwesomeIcons.Moon;
        }
    }
}