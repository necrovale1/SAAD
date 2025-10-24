namespace SAAD.Models
{
    public class User
    {
        public string Uid { get; set; } // preenchido manualmente após leitura do Firebase
        public string Nome { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; } // "Aluno", "Professor", etc.
        public string FaceImageBase64 { get; set; } // imagem facial em base64
    }
}
