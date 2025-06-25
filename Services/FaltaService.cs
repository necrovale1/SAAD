using SAAD.Models;
using System.Collections.ObjectModel;

namespace SAAD.Services
{
    public class FaltaService
    {
        private static FaltaService? _instance;
        public static FaltaService Instance => _instance ??= new FaltaService();

        public ObservableCollection<Faltas> ListaFaltas { get; }

        private FaltaService()
        {
            // Carga inicial de dados de exemplo
            ListaFaltas = new ObservableCollection<Faltas>
            {
                new Faltas { Materia = "Engenharia de Software", Falta = 2, Presenca = 28 },
                new Faltas { Materia = "Banco de Dados", Falta = 4, Presenca = 26 }
            };
        }

        public void AddFalta(Faltas falta)
        {
            if (falta != null)
            {
                ListaFaltas.Add(falta);
            }
        }
    }
}