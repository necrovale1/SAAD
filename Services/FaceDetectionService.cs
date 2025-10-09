namespace SAAD.Services
{
    public class FaceDetectionService
    {
        public async Task<bool> CompararImagensAsync(string base64Referencia, string base64Atual)
        {
            await Task.Delay(1); // Simula operação assíncrona
            return base64Referencia == base64Atual;
        }

    }
}
