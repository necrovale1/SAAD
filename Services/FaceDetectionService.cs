using Newtonsoft.Json.Linq;
using SAAD.Helpers;
using System.Diagnostics;
using System.Text;

namespace SAAD.Services
{
    public class FaceDetectionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private const string ApiBaseUrl = "https://api-us.faceplusplus.com/facepp/v3/";

        public FaceDetectionService()
        {
            _httpClient = new HttpClient();
            _apiKey = SecretsManager.FaceApiKey;
            _apiSecret = SecretsManager.FaceApiSecret;
        }

        /// <summary>
        /// Compara duas imagens (base64) e verifica se são da mesma pessoa.
        /// </summary>
        /// <param name="base64Referencia">A primeira foto (ex: a foto de cadastro)</param>
        /// <param name="base64Atual">A segunda foto (ex: a foto da webcam)</param>
        /// <returns>True se a confiança for alta, false caso contrário</returns>
        public async Task<bool> CompararImagensAsync(string base64Referencia, string base64Atual)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_apiSecret))
                {
                    Debug.WriteLine("Erro de Serviço Facial: Chaves do Face++ não carregadas.");
                    throw new InvalidOperationException("Chaves do Face++ não encontradas no secrets.json.");
                }

                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(_apiKey), "api_key");
                formData.Add(new StringContent(_apiSecret), "api_secret");

                // Envia as duas imagens
                formData.Add(new StringContent(base64Referencia), "image_base64_1");
                formData.Add(new StringContent(base64Atual), "image_base64_2");

                var response = await _httpClient.PostAsync($"{ApiBaseUrl}compare", formData);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Erro na API /compare: {await response.Content.ReadAsStringAsync()}");
                    return false;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseString);

                // Obtém o "nível de confiança" da comparação
                var confidence = json["confidence"]?.ToObject<double>();

                if (confidence == null)
                {
                    Debug.WriteLine("Não foi possível obter a confiança da comparação.");
                    return false;
                }

                // Definimos um "limiar" (threshold).
                // Se a confiança for maior que 80%, consideramos que é a mesma pessoa.
                const double limiar = 80.0;

                Debug.WriteLine($"Confiança da comparação: {confidence} (Limiar: {limiar})");
                return confidence >= limiar;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro em CompararImagensAsync: {ex.Message}");
                return false;
            }
        }
    }
}