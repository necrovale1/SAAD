using Firebase.Database;
using Firebase.Database.Query;
using SAAD2.Models;
using System.Collections.ObjectModel;
using System.Linq; // Adicione este using

namespace SAAD2.Services
{
    public class MateriaService
    {
        private static MateriaService _instance;
        public static MateriaService Instance => _instance ??= new MateriaService();

        private readonly FirebaseClient firebaseClient;
        public ObservableCollection<Materia> Materias { get; private set; }

        private MateriaService()
        {
            firebaseClient = new FirebaseClient("https://saad-1fd38-default-rtdb.firebaseio.com/");
            Materias = new ObservableCollection<Materia>();
        }

        public async Task LoadMateriasAsync()
        {
            var userUid = Preferences.Get("UserUid", string.Empty);
            if (string.IsNullOrWhiteSpace(userUid))
            {
                Materias.Clear();
                return;
            }

            // A consulta foi modificada para capturar a chave
            var materiasFirebase = await firebaseClient
                .Child("materias")
                .Child(userUid)
                .OnceAsync<Materia>();

            Materias.Clear();
            foreach (var item in materiasFirebase)
            {
                var materia = item.Object;
                materia.Key = item.Key; // Guarda a chave única do Firebase
                Materias.Add(materia);
            }
        }

        public async Task AddMateriaAsync(Materia materia)
        {
            var userUid = Preferences.Get("UserUid", string.Empty);
            if (string.IsNullOrWhiteSpace(userUid)) return;

            await firebaseClient.Child("materias").Child(userUid).PostAsync(materia);
            // Recarrega a lista para obter a nova matéria com a sua chave
            await LoadMateriasAsync(); 
        }

        // --- NOVO MÉTODO PARA EXCLUIR ---
        public async Task DeleteMateriaAsync(Materia materia)
        {
            var userUid = Preferences.Get("UserUid", string.Empty);
            if (string.IsNullOrWhiteSpace(userUid) || string.IsNullOrEmpty(materia.Key)) return;

            // Usa a chave da matéria para excluí-la
            await firebaseClient
                .Child("materias")
                .Child(userUid)
                .Child(materia.Key)
                .DeleteAsync();

            // Remove da lista local
            var materiaParaRemover = Materias.FirstOrDefault(m => m.Key == materia.Key);
            if (materiaParaRemover != null)
            {
                Materias.Remove(materiaParaRemover);
            }
        }
        public async Task UpdateMateriaAsync(Materia materia)
        {
            var userUid = Preferences.Get("UserUid", string.Empty);
            if (string.IsNullOrWhiteSpace(userUid) || string.IsNullOrEmpty(materia.Key)) return;

            await firebaseClient
                .Child("materias")
                .Child(userUid)
                .Child(materia.Key)
                .PutAsync(materia);

            await LoadMateriasAsync();
        }
    }
}