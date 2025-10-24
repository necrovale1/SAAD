using System;

namespace SAAD.Models
{
    public class Horario
    {
        public string MateriaKey { get; set; } // chave única da matéria
        public string MateriaName { get; set; }
        public DayOfWeek DiaDaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFim { get; set; }
    }
}
