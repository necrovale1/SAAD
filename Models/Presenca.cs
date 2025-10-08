using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAAD2.Models
{
    public class Presenca
    {
        public string Key { get; set; } // Chave do registo de presença
        public string StudentUid { get; set; }
        public string StudentName { get; set; }
        public string HorarioKey { get; set; } // Chave do horário para saber a aula
        public string MateriaName { get; set; }
        public DateTime Data { get; set; } // A data do registo
        public DateTime? HoraEntrada { get; set; } // Hora de entrada
        public DateTime? HoraSaida { get; set; }   // Hora de saída (pode ser nula)
    }
}