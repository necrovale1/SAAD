using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAAD2.Services
{
    using SAAD2.Helpers;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json;

    public class VisionService
    {
        private readonly string Endpoint = $"https://vision.googleapis.com/v1/images:annotate?key={SecretsManager.VisionApiKey}";
        private readonly HttpClient _httpClient;

        public async Task<List<EntityAnnotation>> AnalyzeImageAsync(string base64Image)
        {
            var request = new VisionRequest
            {
                requests = new List<RequestItem>
            {
                new RequestItem
                {
                    image = new Image { content = base64Image },
                    features = new List<Feature>
                    {
                        new Feature { type = "LABEL_DETECTION", maxResults = 10 }
                    }
                }
            }
            };

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync(Endpoint, request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<VisionResponse>(json);
                return result?.responses?.FirstOrDefault()?.labelAnnotations ?? new List<EntityAnnotation>();
            }

            return new List<EntityAnnotation>();
        }
    }
}