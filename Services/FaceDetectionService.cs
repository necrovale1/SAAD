using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace SAAD.Services
{
    public class FaceDetectionService
    {
        public async Task<bool> CompararImagensAsync(string base64Referencia, string base64Atual)
        {
            // Simulação simples: compara se as strings são iguais (substitua por API real)
            return base64Referencia == base64Atual;
        }
    }
}
