using Firebase.Database;
using Firebase.Database.Query;
using SAAD2.Models;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace SAAD2.Views
{
    public partial class ListagemFacialPage : ContentPage
    {
        private readonly FirebaseClient firebaseClient;
        public ObservableCollection<AlunoFacialStatus> Alunos { get; set; }

        public ListagemFacialPage()
        {
            InitializeComponent();
            firebaseClient = new FirebaseClient("https://saad-1fd38-default-rtdb.firebaseio.com/");
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

