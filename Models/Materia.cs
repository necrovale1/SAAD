using System;

namespace SAAD.Models
{
    public class Materia
    {
        public string MateriaKey { get; set; }     // Chave única da matéria
        public string MateriaName { get; set; }    // Nome da matéria
        public string ProfessorUid { get; set; }   // UID do professor responsável
        public string ProfessorName { get; set; }  // Nome do professor
        public string Curso { get; set; }          // Nome do curso ou turma
    }
}
