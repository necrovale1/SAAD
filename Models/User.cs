using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAAD2.Models
{
    public class User
    {
        //Uid do usuario (Firebase Authentication)
        public string Uid { get; set; }

        //registro facial do usuario em base64
        public string FaceImageBase64 { get; set; } // nova propriedade


        // cargo do usuario (Aluno, Professor, Administrador)
        public string UserType { get; set; }

        // Informações Pessoais
        public string Nome { get; set; }
        public string Email { get; set; }
        public int Idade { get; set; }
        public string Telefone { get; set; }
        public string Sexo { get; set; }
        public string Rg { get; set; }
        public string Cpf { get; set; }
        public string FotoPerfilUrl { get; set; }

        // Informações Acadêmicas
        public string RegistroAcademico { get; set; }
        public string Instituicao { get; set; }
        public string Curso { get; set; }
        public int Semestre { get; set; }
        public string Periodo { get; set; }

        // Acessibilidade
        public string TipoNecessidadeEspecial { get; set; }
        public string Observacoes { get; set; }
    }
}

