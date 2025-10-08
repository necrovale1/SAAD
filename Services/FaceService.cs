using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace SAAD.Services
{
    public class FaceService
    {
        private const string Endpoint = "https://saad-cmd.cognitiveservices.azure.com/";
        private const string SubscriptionKey = "6HnSsXb56V8VpqxYoptwBIpjsbuVbon2nAkytyM7rbXrjYFVmePXJQQJ99BJACZoyfiXJ3w3AAAKACOGDTn2";

        private readonly HttpClient _httpClient;

        public FaceService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
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
