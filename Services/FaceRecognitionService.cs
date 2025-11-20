using Newtonsoft.Json.Linq;
using SAAD.Helpers;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace SAAD.Services
{
    public class FaceRecognitionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiSecret;

        // ID fixo e simples para garantir funcionamento
        private readonly string _faceSetOuterId = "alunosetec2025";

        private const string ApiBaseUrl = "https://api-us.faceplusplus.com/facepp/v3/";

        public FaceRecognitionService()
        {
            _httpClient = new HttpClient();
            // .Trim() remove espaços acidentais no início/fim das chaves
            _apiKey = SecretsManager.FaceApiKey?.Trim();
            _apiSecret = SecretsManager.FaceApiSecret?.Trim();

            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_apiSecret))
            {
                Debug.WriteLine("Erro de Serviço Facial: Chaves do Face++ não carregadas.");
                throw new InvalidOperationException("Chaves do Face++ não encontradas no secrets.json.");
            }
        }

        #region 1. Verificação (Splash Page)

        public async Task CriarGrupoSeNaoExistirAsync()
        {
            try
            {
                Debug.WriteLine($"Tentando criar/verificar FaceSet: {_faceSetOuterId}");

                // MUDANÇA: Tenta CRIAR diretamente usando FormUrlEncoded (mais seguro para texto)
                // Se já existir, a API retorna erro FACESET_EXIST, que nós tratamos como sucesso.
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("api_key", _apiKey),
                    new KeyValuePair<string, string>("api_secret", _apiSecret),
                    new KeyValuePair<string, string>("outer_id", _faceSetOuterId),
                    new KeyValuePair<string, string>("display_name", "Alunos ETEC")
                });

                var response = await _httpClient.PostAsync($"{ApiBaseUrl}faceset/create", formData);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"FaceSet '{_faceSetOuterId}' criado com sucesso.");
                }
                else
                {
                    // Se der erro, verificamos se é porque já existe
                    var errorJson = JObject.Parse(responseString);
                    var errorMessage = errorJson["error_message"]?.ToString();

                    if (errorMessage == "FACESET_EXIST")
                    {
                        Debug.WriteLine($"FaceSet '{_faceSetOuterId}' já existe. A continuar...");
                        // Isto é um "Sucesso" para nós
                    }
                    else
                    {
                        // Qualquer outro erro (ex: INVALID_OUTER_ID real, AUTHENTICATION_ERROR)
                        Debug.WriteLine($"Erro Crítico ao Criar FaceSet: {errorMessage}");
                        throw new Exception($"Erro API Face++: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro em CriarGrupoSeNaoExistirAsync: {ex.Message}");
                throw;
            }
        }

        // O método CriarFaceSetAsync antigo foi removido pois foi fundido com o de cima

        #endregion

        #region 2. Funcionalidades Principais

        // Para envio de imagens, continuamos a usar Multipart, mas com a proteção de aspas

        public async Task<bool> AdicionarRostoAoFaceSetAsync(Stream imagemStream, string alunoId)
        {
            try
            {
                string faceToken = await DetectarRostoAsync(imagemStream);
                if (string.IsNullOrEmpty(faceToken)) return false;

                var formData = new MultipartFormDataContent();
                AdicionarCampo(formData, _apiKey, "api_key");
                AdicionarCampo(formData, _apiSecret, "api_secret");
                AdicionarCampo(formData, _faceSetOuterId, "outer_id");
                AdicionarCampo(formData, faceToken, "face_tokens");

                var response = await _httpClient.PostAsync($"{ApiBaseUrl}faceset/addface", formData);
                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Falha ao adicionar: {await response.Content.ReadAsStringAsync()}");
                    return false;
                }

                var setUserIdForm = new MultipartFormDataContent();
                AdicionarCampo(setUserIdForm, _apiKey, "api_key");
                AdicionarCampo(setUserIdForm, _apiSecret, "api_secret");
                AdicionarCampo(setUserIdForm, faceToken, "face_token");
                AdicionarCampo(setUserIdForm, alunoId, "user_id");

                var setUserIdResponse = await _httpClient.PostAsync($"{ApiBaseUrl}face/setuserid", setUserIdForm);
                return setUserIdResponse.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro em AdicionarRosto: {ex.Message}");
                return false;
            }
        }

        public async Task<string> ReconhecerRostoAsync(Stream imagemStream)
        {
            try
            {
                var base64Image = await ConvertStreamToBase64(imagemStream);

                var formData = new MultipartFormDataContent();
                AdicionarCampo(formData, _apiKey, "api_key");
                AdicionarCampo(formData, _apiSecret, "api_secret");
                AdicionarCampo(formData, _faceSetOuterId, "outer_id");
                AdicionarCampo(formData, base64Image, "image_base64");

                var response = await _httpClient.PostAsync($"{ApiBaseUrl}search", formData);

                if (!response.IsSuccessStatusCode) return null;

                var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                var results = json["results"];
                if (results == null || !results.HasValues) return null;

                var bestMatch = results.OrderByDescending(r => (double)r["confidence"]).FirstOrDefault();
                if (bestMatch == null) return null;

                double confidence = (double)bestMatch["confidence"];
                string userId = bestMatch["user_id"]?.ToString();

                if (confidence > 80.0 && !string.IsNullOrEmpty(userId)) return userId;

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro em ReconhecerRosto: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Métodos Auxiliares

        private async Task<string> DetectarRostoAsync(Stream imagemStream)
        {
            var base64Image = await ConvertStreamToBase64(imagemStream);

            var formData = new MultipartFormDataContent();
            AdicionarCampo(formData, _apiKey, "api_key");
            AdicionarCampo(formData, _apiSecret, "api_secret");
            AdicionarCampo(formData, base64Image, "image_base64");

            var response = await _httpClient.PostAsync($"{ApiBaseUrl}detect", formData);
            if (!response.IsSuccessStatusCode) return null;

            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            var faces = json["faces"];
            return (faces != null && faces.HasValues) ? faces[0]["face_token"]?.ToString() : null;
        }

        /// <summary>
        /// Garante aspas nos campos multipart para evitar erro "missing argument"
        /// </summary>
        private void AdicionarCampo(MultipartFormDataContent form, string valor, string nomeCampo)
        {
            var content = new StringContent(valor);
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = $"\"{nomeCampo}\""
            };
            form.Add(content);
        }

        private async Task<string> ConvertStreamToBase64(Stream stream)
        {
            if (stream is MemoryStream ms) return Convert.ToBase64String(ms.ToArray());
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        #endregion
    }
}