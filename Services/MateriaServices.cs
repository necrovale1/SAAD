using Firebase.Database;
using Firebase.Database.Query;
using SAAD2.Models;
using System.Collections.ObjectModel;

namespace SAAD2.Services
{
    public class MateriaService
    {
        private static MateriaService _instance;
        public static MateriaService Instance => _instance ??= new MateriaService();

        private readonly FirebaseClient firebaseClient;
        public ObservableCollection<Materia> Materias { get; private set; }
        private bool isLoaded = false;

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

            if (isLoaded) return;

            // --- MODIFIQUE A CONSULTA AQUI ---
            var materias = await firebaseClient
                .Child("materias")
                .Child(userUid) // Adiciona o ID do usuário ao caminho
                .OnceAsync<Materia>();
            // ---------------------------------

            Materias.Clear();
            foreach (var materia in materias)
            {
                Materias.Add(materia.Object);
            }
            isLoaded = true;
        }

        public async Task AddMateriaAsync(Materia materia)
        {
            var userUid = Preferences.Get("UserUid", string.Empty);
            if (string.IsNullOrWhiteSpace(userUid)) return;

            await firebaseClient.Child("materias").Child(userUid).PostAsync(materia);
            Materias.Add(materia);
        }
    }
}