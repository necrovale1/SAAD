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

        public RegistroPresencaPage()
        {
            InitializeComponent();

            firebaseClient = new FirebaseClient(
                SecretsManager.FirebaseUrl,
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(SecretsManager.FirebaseSecret)
                });

            WeakReferenceMessenger.Default.Register<AlunoReconhecidoMessage>(this, async (r, msg) =>
            {
                await HandlePresenceRegistration(msg.Value, isEntrada: true);
            });
        }

        private async void OnRegistrarPresencaClicked(object sender, EventArgs e)
        {
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
                .FirstOrDefault(p => p.Object.StudentUid == aluno.Uid &&
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

        private async void OnRegistrarSaidaClicked(object sender, EventArgs e)
        {
            // Envia para CameraCapturePage com isEntrada: false
            WeakReferenceMessenger.Default.Register<AlunoReconhecidoMessage>(this, async (r, msg) =>
            {
                if (!msg.IsEntrada)
                    await HandlePresenceRegistration(msg.Value, isEntrada: false);
            });

            await Navigation.PushAsync(new CameraCapturePage());
        }
    }
}
