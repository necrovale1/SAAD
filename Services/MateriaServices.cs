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

namespace SAAD2.Services;
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
        LoadMaterias();
    }

    private async void LoadMaterias()
    {
        var materias = await firebaseClient
            .Child("materias")
            .OnceAsync<Materia>();

        foreach (var materia in materias)
        {
            Materias.Add(materia.Object);
        }
    }

    public async void AddMateria(Materia materia)
    {
        await firebaseClient.Child("materias").PostAsync(materia);
        Materias.Add(materia);
    }
}