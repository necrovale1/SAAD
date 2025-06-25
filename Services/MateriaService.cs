using SAAD.Models;
using System.Collections.ObjectModel;

namespace SAAD.Services
{
    public class MateriaService
    {
        // 1. Correção: Declarar o campo como nullable (?) para resolver o aviso do compilador.
        private static MateriaService? _instance;

        // A lógica de inicialização aqui já garante que a propriedade nunca retorne nulo.
        public static MateriaService Instance => _instance ??= new MateriaService();

        public ObservableCollection<Materias> Materias { get; }

        private MateriaService()
        {
            Materias = new ObservableCollection<Materias>
            {
                new Materias { Nome = "Engenharia de Software", Descricao = "Estudo dos processos de desenvolvimento de software.", Categoria = "Computação" },
                new Materias { Nome = "Banco de Dados", Descricao = "Conceitos e práticas de sistemas de gerenciamento de bancos de dados.", Categoria = "Dados" }
            };
        }

        public void AddMateria(Materias materia)
        {
            if (materia != null)
            {
                Materias.Add(materia);
            }
        }
    }
}