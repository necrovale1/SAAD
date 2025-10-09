using SAAD.Helpers;

namespace SAAD.Models
{
    public class AlunoFacialStatus
    {
        public string Nome { get; set; }
        public string RegistroAcademico { get; set; }

        // Ícone FontAwesome para status facial
        public string IconFacial => TemImagemFacial ? FontAwesomeIcons.Check : FontAwesomeIcons.Times;

        // Cor do ícone
        public Color CorFacial => TemImagemFacial ? Colors.Green : Colors.Red;

        // Status interno
        public bool TemImagemFacial { get; set; }
    }
}
