using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAAD2.Views;
using SAAD2.Helpers;
using SAAD2.Models;
using System.Collections.ObjectModel;
using Firebase.Database;
using Firebase.Database.Query;

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

        public async Task LoadFaltasAsync()
        {
            if (isLoaded) return; // Carrega os dados apenas uma vez

            var faltas = await firebaseClient
                .Child("faltas")
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
            await firebaseClient.Child("faltas").PostAsync(falta);
            Faltas.Add(falta);
        }
    }
}