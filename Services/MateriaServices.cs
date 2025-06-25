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
    public class MateriaService
    {
        private static MateriaService _instance;
        public static MateriaService Instance => _instance ??= new MateriaService();

        public ObservableCollection<Materia> Materias { get; private set; }

        private MateriaService()
        {
            Materias = new ObservableCollection<Materia>
            {
                new Materia { Nome = "Engenharia de Software", Descricao = "Estudo de metodologias e práticas para desenvolvimento de software.", Categoria = "Obrigatória" },
                new Materia { Nome = "Banco de Dados", Descricao = "Conceitos e práticas de sistemas de gerenciamento de bancos de dados.", Categoria = "Obrigatória" }
            };
        }

        public void AddMateria(Materia materia)
        {
            Materias.Add(materia);
        }
    }
}