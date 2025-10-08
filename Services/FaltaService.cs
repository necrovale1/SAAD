using Firebase.Database;
using Firebase.Database.Query;
using SAAD2.Models;
using System.Collections.ObjectModel;
using System.Linq; // Adicione este using

namespace SAAD2.Services
{
    public class FaltaService
    {
        private static FaltaService _instance;
        public static FaltaService Instance => _instance ??= new FaltaService();

        private readonly FirebaseClient firebaseClient;
        public ObservableCollection<Falta> Faltas { get; private set; }

        private FaltaService()
        {
            firebaseClient = new FirebaseClient("https://saad-1fd38-default-rtdb.firebaseio.com/");
            Faltas = new ObservableCollection<Falta>();
        }

        public async Task LoadFaltasAsync()
        {
            var userUid = Preferences.Get("UserUid", string.Empty);
            if (string.IsNullOrWhiteSpace(userUid))
            {
                Faltas.Clear();
                return;
            }

            // A consulta foi modificada para capturar a chave
            var faltasFirebase = await firebaseClient
                .Child("faltas") // Assumindo que os dados de faltas estão em "faltas"
                .Child(userUid)
                .OnceAsync<Falta>();

            Faltas.Clear();
            foreach (var item in faltasFirebase)
            {
                var falta = item.Object;
                falta.Key = item.Key; // Guarda a chave única do Firebase
                Faltas.Add(falta);
            }
        }

        public async Task AddFaltaAsync(Falta falta)
        {
            var userUid = Preferences.Get("UserUid", string.Empty);
            if (string.IsNullOrWhiteSpace(userUid)) return;

            await firebaseClient.Child("faltas").Child(userUid).PostAsync(falta);
            // Recarrega a lista para obter a nova falta com a sua chave
            await LoadFaltasAsync();
        }

        // --- NOVO MÉTODO PARA EXCLUIR ---
        public async Task DeleteFaltaAsync(Falta falta)
        {
            var userUid = Preferences.Get("UserUid", string.Empty);
            if (string.IsNullOrWhiteSpace(userUid) || string.IsNullOrEmpty(falta.Key)) return;

            // Usa a chave da falta para excluí-la
            await firebaseClient
                .Child("faltas")
                .Child(userUid)
                .Child(falta.Key)
                .DeleteAsync();

            // Remove da lista local
            var faltaParaRemover = Faltas.FirstOrDefault(f => f.Key == falta.Key);
            if (faltaParaRemover != null)
            {
                Faltas.Remove(faltaParaRemover);
            }
        }

        public async Task UpdateFaltaAsync(Falta falta)
        {
            var userUid = Preferences.Get("UserUid", string.Empty);
            if (string.IsNullOrWhiteSpace(userUid) || string.IsNullOrEmpty(falta.Key)) return;

            await firebaseClient
                .Child("faltas")
                .Child(userUid)
                .Child(falta.Key)
                .PutAsync(falta);

            await LoadFaltasAsync();
        }
    }
}
