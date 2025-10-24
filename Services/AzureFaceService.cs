using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using SAAD.Helpers; // para acessar SecretsManager

namespace SAAD.Services
{
    public static class AzureFaceService
    {
        private static readonly string subscriptionKey = SecretsManager.FaceServiceKey;
        private static readonly string endpoint = SecretsManager.FaceServiceEndpoint;

        private static readonly string detectUrl = $"{endpoint}/face/v1.0/detect?returnFaceId=true";
        private static readonly string verifyUrl = $"{endpoint}/face/v1.0/verify";

        private static readonly HttpClient client = new HttpClient();

        public static async Task<bool> CompararAsync(Stream imagemCapturada, Stream imagemCadastrada)
        {
            var faceId1 = await DetectFaceIdAsync(imagemCapturada);
            var faceId2 = await DetectFaceIdAsync(imagemCadastrada);

            if (faceId1 == null || faceId2 == null)
                return false;

            var payload = new
            {
                faceId1,
                faceId2
            };

            var content = new StringContent(JsonSerializer.Serialize(payload));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            var response = await client.PostAsync(verifyUrl, content);
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return root.GetProperty("isIdentical").GetBoolean();
        }

        private static async Task<string> DetectFaceIdAsync(Stream imageStream)
        {
            var content = new StreamContent(imageStream);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            var response = await client.PostAsync(detectUrl, content);
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.GetArrayLength() == 0)
                return null;

            return root[0].GetProperty("faceId").GetString();
        }
    }
}
