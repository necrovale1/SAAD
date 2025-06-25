using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAAD.Models
{
    public class Faltas
    {
        public required string Materia { get; set; }
        public int Falta { get; set; }
        public int Presenca { get; set; }
    }
}