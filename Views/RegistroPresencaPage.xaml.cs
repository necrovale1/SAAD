using Firebase.Database;
using Firebase.Database.Query;
using SAAD.Helpers;
using SAAD.Models;
using SAAD.Services;
using SAAD.Messages;
using SAAD.Resources.Styles;
using CommunityToolkit.Mvvm.Messaging;
using UserModel = SAAD.Models.User;

namespace SAAD.Views
{
    public partial class RegistroPresencaPage : ContentPage
    {
        private readonly FirebaseClient firebaseClient;
        private bool _proximaCapturaEEntrada = true;

        public RegistroPresencaPage()
        {
            InitializeComponent();

            // Atualiza o ícone do botão que agora está na TitleView
            ThemeButton.Text = Application.Current.UserAppTheme == AppTheme.Dark
                ? FontAwesomeIcons.Sun
                : FontAwesomeIcons.Moon;

            firebaseClient = new FirebaseClient(
                SecretsManager.FirebaseUrl,
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(SecretsManager.FirebaseSecret)
                });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            WeakReferenceMessenger.Default.Register<AlunoReconhecidoMessage>(this, async (r, msg) =>
            {
                await HandlePresenceRegistration(msg.Value, isEntrada: _proximaCapturaEEntrada);
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            WeakReferenceMessenger.Default.Unregister<AlunoReconhecidoMessage>(this);
        }

        // --- 1. CORREÇÃO DO CRASH (Permissões) ---
        private async Task<bool> VerificarPermissaoCamera()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }

            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permissão Necessária", "O app precisa da câmera para registrar presença.", "OK");
                return false;
            }
            return true;
        }

        private async void OnRegistrarPresencaClicked(object sender, EventArgs e)
        {
            if (!await VerificarPermissaoCamera()) return;
            _proximaCapturaEEntrada = true;
            await Navigation.PushAsync(new CameraCapturePage());
        }

        private async void OnRegistrarSaidaClicked(object sender, EventArgs e)
        {
            if (!await VerificarPermissaoCamera()) return;
            _proximaCapturaEEntrada = false;
            await Navigation.PushAsync(new CameraCapturePage());
        }

        private async void OnCadastrarAlunoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CadastroAlunoPage());
        }

        // --- 3. LÓGICA DE TROCA DE TEMA (FontAwesome Atualizado) ---
        private void OnThemeSwitchClicked(object sender, EventArgs e)
        {
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                var currentTheme = mergedDictionaries.FirstOrDefault(d => d is DarkTheme || d is LightTheme);
                if (currentTheme != null)
                {
                    mergedDictionaries.Remove(currentTheme);
                }

                if (Application.Current.UserAppTheme == AppTheme.Dark)
                {
                    Application.Current.UserAppTheme = AppTheme.Light;
                    mergedDictionaries.Add(new LightTheme());
                    ThemeButton.Text = FontAwesomeIcons.Moon; // Ícone Lua (FontAwesome)
                }
                else
                {
                    Application.Current.UserAppTheme = AppTheme.Dark;
                    mergedDictionaries.Add(new DarkTheme());
                    ThemeButton.Text = FontAwesomeIcons.Sun; // Ícone Sol (FontAwesome)
                }
            }
        }

        // --- LÓGICA DE REGISTRO (MANTIDA) ---
        private async Task HandlePresenceRegistration(UserModel aluno, bool isEntrada)
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                var aulaAtual = await GetCurrentClass();

                if (aulaAtual == null)
                {
                    bool continuar = await DisplayAlert("Aviso", "Não há aula agora. Deseja registrar mesmo assim?", "Sim", "Não");
                    if (!continuar) return;
                    aulaAtual = new Horario { MateriaKey = "extra", MateriaName = "Horário Extra" };
                }

                if (isEntrada)
                    await RegisterEntrada(aluno, aulaAtual);
                else
                    await RegisterSaida(aluno);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao registrar: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }

        private async Task<Horario> GetCurrentClass()
        {
            try
            {
                var horarios = await firebaseClient.Child("horarios").OnceAsync<Horario>();
                var today = DateTime.Now.DayOfWeek;
                var now = DateTime.Now.TimeOfDay;

                return horarios
                    .Select(h => h.Object)
                    .FirstOrDefault(h => h.DiaDaSemana == today && h.HoraInicio <= now && h.HoraFim >= now);
            }
            catch { return null; }
        }

        private async Task RegisterEntrada(UserModel aluno, Horario aula)
        {
            var presencasHoje = await firebaseClient.Child("presencas").OnceAsync<Presenca>();
            var entradaAberta = presencasHoje
                .FirstOrDefault(p => p.Object.StudentUid == aluno.Id && p.Object.Data.Date == DateTime.Today && p.Object.HoraSaida == null);

            if (entradaAberta != null)
            {
                await DisplayAlert("Aviso", $"Entrada já registrada às {entradaAberta.Object.HoraEntrada:HH:mm}.", "OK");
                return;
            }

            var novaPresenca = new Presenca
            {
                StudentUid = aluno.Id,
                StudentName = aluno.Nome,
                HorarioKey = aula.MateriaKey,
                MateriaName = aula.MateriaName,
                Data = DateTime.Today,
                HoraEntrada = DateTime.Now
            };

            await firebaseClient.Child("presencas").PostAsync(novaPresenca);
            await DisplayAlert("Sucesso", $"Entrada de {aluno.Nome} confirmada!", "OK");
        }

        private async Task RegisterSaida(UserModel aluno)
        {
            var presencas = await firebaseClient.Child("presencas").OnceAsync<Presenca>();
            var entradaAberta = presencas
                .FirstOrDefault(p => p.Object.StudentUid == aluno.Id && p.Object.Data.Date == DateTime.Today && p.Object.HoraSaida == null);

            if (entradaAberta == null)
            {
                await DisplayAlert("Erro", "Não encontrei uma entrada aberta para fechar hoje.", "OK");
                return;
            }

            entradaAberta.Object.HoraSaida = DateTime.Now;
            await firebaseClient.Child("presencas").Child(entradaAberta.Key).PutAsync(entradaAberta.Object);
            await DisplayAlert("Sucesso", $"Saída registrada às {entradaAberta.Object.HoraSaida:HH:mm}.", "OK");
        }
    }
}