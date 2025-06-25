using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAAD2.Views;
using SAAD2.Helpers;
using SAAD2.Models;

using System.Collections.ObjectModel;

namespace SAAD2.Services
{
    public class FaltaService
    {
        private static FaltaService _instance;
        public static FaltaService Instance => _instance ??= new FaltaService();

        public ObservableCollection<Falta> Faltas { get; private set; }

        private FaltaService()
        {
            Faltas = new ObservableCollection<Falta>
            {
                new Falta { Materia = "Engenharia de Software", Faltas = 2, Presencas = 18 },
                new Falta { Materia = "Banco de Dados", Faltas = 1, Presencas = 19 }
            };
        }

        public void AddFalta(Falta falta)
        {
            Faltas.Add(falta);
        }
    }
}