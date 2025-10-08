using Firebase.Database;
using Firebase.Database.Query;
using SAAD2.Models;
using System.Linq;
using UserModel = SAAD2.Models.User;
using SAAD2.Enums;
using SAAD2.Helpers;

namespace SAAD2.Views
{
    public partial class RegistroPresencaPage : ContentPage
    {
        private readonly FirebaseClient firebaseClient;

        public RegistroPresencaPage()
        {
            InitializeComponent();
            firebaseClient = new FirebaseClient("https://saad-1fd38-default-rtdb.firebaseio.com/");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateThemeIcon();
        }
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
                // 1. Encontrar o Aluno pelo RA
                var aluno = await FindStudentByRA(RaEntry.Text);
                if (aluno == null)
                {
                    await DisplayAlert("Erro", "RA não encontrado. Verifique o número digitado.", "OK");
                    return;
                }

                // 2. Encontrar a aula atual baseada no horário
                var aulaAtual = await GetCurrentClass();
                if (aulaAtual == null)
                {
                    await DisplayAlert("Aviso", "Não há nenhuma aula programada para o horário atual.", "OK");
                    return;
                }

                if (isEntrada)
                {
                    // Lógica de ENTRADA
                    await RegisterEntrada(aluno, aulaAtual);
                }
                else
                {
                    // Lógica de SAÍDA
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
                RaEntry.Text = string.Empty; // Limpa o campo para o próximo utilizador
            }
        }

        private async Task<UserModel> FindStudentByRA(string ra)
        {
            var users = await firebaseClient.Child("users").OnceAsync<UserModel>();
            return users
                .Where(u => u.Object.RegistroAcademico == ra && u.Object.UserType == "Aluno")
                .Select(u =>
                {
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
            // Verifica se já não há uma entrada aberta para este aluno hoje
            var presencasHoje = await firebaseClient.Child("presencas").OnceAsync<Presenca>();
            var entradaAberta = presencasHoje
                .FirstOrDefault(p => p.Object.StudentUid == aluno.Uid && p.Object.Data.Date == DateTime.Today && p.Object.HoraSaida == null);

            if (entradaAberta != null)
            {
                await DisplayAlert("Aviso", $"{aluno.Nome}, a sua entrada já foi registrada às {entradaAberta.Object.HoraEntrada:HH:mm}.", "OK");
                return;
            }

            var novaPresenca = new Presenca
            {
                StudentUid = aluno.Uid,
                StudentName = aluno.Nome,
                HorarioKey = aula.MateriaKey, // Assumindo que o Horario terá uma Key
                MateriaName = aula.MateriaName,
                Data = DateTime.Today,
                HoraEntrada = DateTime.Now
            };

            await firebaseClient.Child("presencas").PostAsync(novaPresenca);
            await DisplayAlert("Sucesso!", $"Entrada de {aluno.Nome} registrada com sucesso às {novaPresenca.HoraEntrada:HH:mm} para a aula de {aula.MateriaName}.", "OK");
        }

        private async Task RegisterSaida(UserModel aluno)
        {
            var presencas = await firebaseClient.Child("presencas").OnceAsync<Presenca>();
            var entradaAberta = presencas
                .FirstOrDefault(p => p.Object.StudentUid == aluno.Uid && p.Object.Data.Date == DateTime.Today && p.Object.HoraSaida == null);

            if (entradaAberta == null)
            {
                await DisplayAlert("Erro", "Nenhum registro de entrada aberto foi encontrado para você hoje. Por favor, registre sua entrada primeiro.", "OK");
                return;
            }

            entradaAberta.Object.HoraSaida = DateTime.Now;
            await firebaseClient.Child("presencas").Child(entradaAberta.Key).PutAsync(entradaAberta.Object);
            await DisplayAlert("Até Logo!", $"Saída de {aluno.Nome} registrada com sucesso às {entradaAberta.Object.HoraSaida:HH:mm}.", "OK");
        }

        private void SetUIState(bool isLoading)
        {
            LoadingIndicator.IsVisible = isLoading;
            LoadingIndicator.IsRunning = isLoading;
            EntradaButton.IsEnabled = !isLoading;
            SaidaButton.IsEnabled = !isLoading;
        }

        private void OnThemeIconTapped(object sender, TappedEventArgs e)
        {
            var app = (App)Application.Current;
            app.ToggleTheme(); // O método que criámos no App.xaml.cs
            UpdateThemeIcon();
        }

        // ADICIONE ESTE MÉTODO
        private void UpdateThemeIcon()
        {
            var app = (App)Application.Current;
            ThemeIconLabel.Text = app.CurrentTheme == Theme.Light
                ? FontAwesomeIcons.Sun
                : FontAwesomeIcons.Moon;
        }
    }
}