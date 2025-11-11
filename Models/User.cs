namespace SAAD.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Uid { get; set; } 
        public string Nome { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public string FotoUrl { get; set; }
        public string AzurePersonId { get; set; }
        public string RegistroAcademico { get; set; }
        public string FaceImageBase64 { get; set; }
    }
}