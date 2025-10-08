using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAAD2.Models
{
    public class Horario
    {
        public string MateriaKey { get; set; }
        public string MateriaName { get; set; }
        public DayOfWeek DiaDaSemana { get; set; } // Usaremos o tipo DayOfWeek (Sunday = 0, Monday = 1, etc.)
        public TimeSpan HoraInicio { get; set; } // Ex: 19:00
        public TimeSpan HoraFim { get; set; }    // Ex: 22:25
    }
}