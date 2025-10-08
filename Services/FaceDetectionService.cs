using System.Text.Json;
using System.Text;

namespace SAAD2.Services
{
    public class FaceDetectionService
    {
        private readonly string _apiKey = "YOUR_API_KEY"; // Substitua pela sua chave da Vision API

        public async Task<(bool hasFace, string message)> DetectFaceAsync(Stream photoStream)
        {
            using var ms = new MemoryStream();
            await photoStream.CopyToAsync(ms);
            var base64Image = Convert.ToBase64String(ms.ToArray());

            var requestBody = new
            {
                requests = new[]
                {
                    new
                    {
                        image = new { content = base64Image },
                        features = new[] { new { type = "FACE_DETECTION", maxResults = 5 } }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            var response = await client.PostAsync(
                $"https://vision.googleapis.com/v1/images:annotate?key={_apiKey}",
                content
            );

            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return (false, $"Erro na API: {response.StatusCode}");

            var result = JsonDocument.Parse(responseJson);
            var faceAnnotations = result.RootElement
                .GetProperty("responses")[0]
                .TryGetProperty("faceAnnotations", out var faces) ? faces : default;

            if (faces.ValueKind != JsonValueKind.Array || faces.GetArrayLength() == 0)
                return (false, "Nenhum rosto detectado.");

            if (faces.GetArrayLength() == 1)
                return (true, "Rosto detectado com sucesso!");

            return (false, $"Foram detectados {faces.GetArrayLength()} rostos. Por favor, envie uma imagem com apenas uma pessoa.");
        }
    }
}
