using Firebase.Database;
using SAAD.Helpers;
using SAAD.Models;
using System.Collections.ObjectModel;

namespace SAAD.Views
{
    public partial class ListagemFacialPage : ContentPage
    {
        private readonly FirebaseClient firebaseClient;
        public ObservableCollection<AlunoFacialStatus> Alunos { get; set; }

        public ListagemFacialPage()
        {
            InitializeComponent();
            var firebaseClient = new FirebaseClient(
            SecretsManager.FirebaseUrl,
            new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(SecretsManager.FirebaseSecret)
            });

            Alunos = new ObservableCollection<AlunoFacialStatus>();
            AlunosCollection.ItemsSource = Alunos;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarAlunos();
        }

        private async Task CarregarAlunos()
        {
            Alunos.Clear();
            var users = await firebaseClient.Child("users").OnceAsync<User>();
            var alunos = users
             .Where(u => u.Object.UserType == "Aluno")
             .Select(u => new AlunoFacialStatus
             {
                 Nome = u.Object.Nome,
                 RegistroAcademico = u.Object.RegistroAcademico,
                 TemImagemFacial = !string.IsNullOrEmpty(u.Object.FaceImageBase64)
             });


            foreach (var aluno in alunos)
                Alunos.Add(aluno);
        }


    }
}

