using System;

namespace SAAD.Models
{
    public class Presenca
    {
        public string StudentUid { get; set; }
        public string StudentName { get; set; }
        public string HorarioKey { get; set; }
        public string MateriaName { get; set; }
        public DateTime Data { get; set; }
        public DateTime HoraEntrada { get; set; }
        public DateTime? HoraSaida { get; set; } // pode ser nulo até o aluno sair
    }
}
