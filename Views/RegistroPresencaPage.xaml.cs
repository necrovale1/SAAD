using Firebase.Database;
using Firebase.Database.Query;
using SAAD.Helpers;
using SAAD.Models;
using SAAD.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.Messaging;
using SAAD.Messages;
using UserModel = SAAD.Models.User;

namespace SAAD.Views
{
    public partial class RegistroPresencaPage : ContentPage
    {
        private FirebaseClient firebaseClient;
        private bool _proximaCapturaEEntrada = true; // Variável para controlar se é entrada ou saída

        public RegistroPresencaPage()
        {
            InitializeComponent();

            firebaseClient = new FirebaseClient(
                SecretsManager.FirebaseUrl,
                new FirebaseOptions
                {
                    // CORREÇÃO: Certifique-se que SecretsManager.FirebaseSecret existe
                    AuthTokenAsyncFactory = () => Task.FromResult(SecretsManager.FirebaseSecret)
                });
        }

        // Inscreva-se no OnAppearing para evitar duplicações
        protected override void OnAppearing()
        {
            base.OnAppearing();
            WeakReferenceMessenger.Default.Register<AlunoReconhecidoMessage>(this, async (r, msg) =>
            {
                // Usa a variável de controle para saber se é entrada ou saída
                await HandlePresenceRegistration(msg.Value, isEntrada: _proximaCapturaEEntrada);
            });
        }

        // Cancele a inscrição no OnDisappearing
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            WeakReferenceMessenger.Default.Unregister<AlunoReconhecidoMessage>(this);
        }

        private async void OnRegistrarPresencaClicked(object sender, EventArgs e)
        {
            _proximaCapturaEEntrada = true; // Define que a próxima leitura é ENTRADA
            await Navigation.PushAsync(new CameraCapturePage());
        }

        private async void OnRegistrarSaidaClicked(object sender, EventArgs e)
        {
            _proximaCapturaEEntrada = false; // Define que a próxima leitura é SAÍDA
            await Navigation.PushAsync(new CameraCapturePage());
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
                await RegisterEntrada(aluno, aulaAtual);
            else
                await RegisterSaida(aluno);
        }

        private async Task<Horario> GetCurrentClass()
        {
            // Nota: Verifique se a estrutura do seu Firebase para "horarios" está correta para esta consulta
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

            // CORREÇÃO: aluno.Uid -> aluno.Id
            var entradaAberta = presencasHoje
                .FirstOrDefault(p => p.Object.StudentUid == aluno.Id && p.Object.Data.Date == DateTime.Today && p.Object.HoraSaida == null);

            if (entradaAberta != null)
            {
                await DisplayAlert("Aviso", $"{aluno.Nome}, sua entrada já foi registrada às {entradaAberta.Object.HoraEntrada:HH:mm}.", "OK");
                return;
            }

            var novaPresenca = new Presenca
            {
                // CORREÇÃO: aluno.Uid -> aluno.Id
                StudentUid = aluno.Id,
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

            // CORREÇÃO: aluno.Uid -> aluno.Id
            var entradaAberta = presencas
                .FirstOrDefault(p => p.Object.StudentUid == aluno.Id &&
                                     p.Object.Data.Date == DateTime.Today &&
                                     p.Object.HoraSaida == null);

            if (entradaAberta == null)
            {
                await DisplayAlert("Erro", "Nenhum registro de entrada aberto encontrado para hoje.", "OK");
                return;
            }

            entradaAberta.Object.HoraSaida = DateTime.Now;
            await firebaseClient.Child("presencas").Child(entradaAberta.Key).PutAsync(entradaAberta.Object);
            await DisplayAlert("Até logo!", $"Saída registrada às {entradaAberta.Object.HoraSaida:HH:mm}.", "OK");
        }
    }
}