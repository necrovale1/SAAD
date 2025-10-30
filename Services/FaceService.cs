using System.Net.Http.Headers;
using System.Net.Http.Json;
using SAAD.Helpers; 

namespace SAAD.Services
{
    public class FaceService
    {
        private const string Endpoint = "https://saad-cmd.cognitiveservices.azure.com/";

        // SUBSTITUA A CHAVE ANTIGA POR ISSO:
        private static string SubscriptionKey => SecretsManager.FaceServiceKey;

        private readonly HttpClient _httpClient;

        public FaceService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
        }

        public async Task<string> AnalyzeImageAsync(Stream imageStream)
        {
            // O endpoint para "Análise de Imagem" é diferente do endpoint de "Face"
            // Pedimos por "tags" (rótulos) e "objects" (objetos) em português
            var url = $"{Endpoint}/vision/v4.0/analyze?features=tags,objects&language=pt";

            using var content = new StreamContent(imageStream);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await _httpClient.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();

            // O formato do JSON será diferente do Google, 
            // você precisará extrair os rótulos (tags) dele
            return json;
        }

        public async Task<string> DetectFaceAsync(Stream imageStream)
        {
            var url = $"{Endpoint}/face/v1.0/detect?returnFaceId=true";

            using var content = new StreamContent(imageStream);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await _httpClient.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();

            // Aqui você pode extrair o faceId do JSON
            return json;
        }

        public async Task<string> IdentifyFaceAsync(string faceId)
        {
            var url = $"{Endpoint}/face/v1.0/identify";

            var body = new
            {
                personGroupId = "alunos",
                faceIds = new[] { faceId },
                maxNumOfCandidatesReturned = 1,
                confidenceThreshold = 0.5
            };

            var response = await _httpClient.PostAsJsonAsync(url, body);
            var json = await response.Content.ReadAsStringAsync();

            // Aqui você pode extrair o RA (personId) do JSON
            return json;
        }
    }
}