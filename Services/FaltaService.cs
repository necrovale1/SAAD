using Firebase.Database;
using Firebase.Database.Query;
using SAAD2.Models;
using System.Collections.ObjectModel;

namespace SAAD2.Services
{
    public class FaltaService
    {
        private static FaltaService _instance;
        public static FaltaService Instance => _instance ??= new FaltaService();

        private readonly FirebaseClient firebaseClient;
        public ObservableCollection<Falta> Faltas { get; private set; }
        private bool isLoaded = false;

        private FaltaService()
        {
            firebaseClient = new FirebaseClient("https://saad-1fd38-default-rtdb.firebaseio.com/");
            Faltas = new ObservableCollection<Falta>();
        }

        public async Task LoadFaltasAsync(bool forceRefresh = false)
        {
            var userUid = Preferences.Get("UserUid", string.Empty);
            if (string.IsNullOrWhiteSpace(userUid))
            {
                Faltas.Clear();
                return;
            }

            // Se não for para forçar, e já estiver carregado, não faz nada
            if (!forceRefresh && isLoaded) return;

            var faltas = await firebaseClient
                .Child("faltas")
                .Child(userUid)
                .OnceAsync<Falta>();

            Faltas.Clear();
            foreach (var falta in faltas)
            {
                Faltas.Add(falta.Object);
            }
            isLoaded = true;
        }

        public async Task AddFaltaAsync(Falta falta)
        {
            var userUid = Preferences.Get("UserUid", string.Empty);
            if (string.IsNullOrWhiteSpace(userUid)) return; // Não salva se não estiver logado

            // Adiciona a falta sob o ID do usuário
            await firebaseClient.Child("faltas").Child(userUid).PostAsync(falta);
            Faltas.Add(falta);
        }
    }
}