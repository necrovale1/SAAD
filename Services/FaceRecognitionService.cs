using Newtonsoft.Json.Linq;
using SAAD.Helpers;
using System.Diagnostics;
using System.Text;

namespace SAAD.Services
{
    public class FaceRecognitionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _faceSetOuterId = SecretsManager.PersonGroupId;

        private const string ApiBaseUrl = "https://api-us.faceplusplus.com/facepp/v3/";

        public FaceRecognitionService()
        {
            _httpClient = new HttpClient();
            _apiKey = SecretsManager.FaceApiKey;
            _apiSecret = SecretsManager.FaceApiSecret;

            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_apiSecret))
            {
                Debug.WriteLine("Erro de Serviço Facial: Chaves do Face++ não carregadas.");
                throw new InvalidOperationException("Chaves do Face++ não encontradas no secrets.json.");
            }
        }

        #region 1. Verificação (Splash Page)

        /// <summary>
        /// Método da SplashPage. Verifica se o FaceSet (Grupo de Alunos) existe.
        /// </summary>
        public async Task CriarGrupoSeNaoExistirAsync()
        {
            try
            {
                // --- INÍCIO DA CORREÇÃO ---
                // Esta chamada não envia ficheiros, por isso usa FormUrlEncodedContent
                // (como sugerido nas suas dicas)
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("api_key", _apiKey),
                    new KeyValuePair<string, string>("api_secret", _apiSecret),
                    new KeyValuePair<string, string>("outer_id", _faceSetOuterId)
                });
                // --- FIM DA CORREÇÃO ---

                var response = await _httpClient.PostAsync($"{ApiBaseUrl}faceset/getdetail", formData);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var errorJson = JObject.Parse(responseString);
                    var errorMessage = errorJson["error_message"]?.ToString();

                    if (errorMessage == "FACESET_NOT_FOUND")
                    {
                        Debug.WriteLine($"FaceSet '{_faceSetOuterId}' não encontrado. A criar...");
                        await CriarFaceSetAsync();
                    }
                    else
                    {
                        Debug.WriteLine($"Erro do Face++ (GetDetail): {errorMessage}");
                        throw new Exception($"Erro da API Face++: {errorMessage}");
                    }
                }
                else
                {
                    Debug.WriteLine($"FaceSet '{_faceSetOuterId}' já existe.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro inesperado em CriarGrupoSeNaoExistirAsync: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Método auxiliar para criar o FaceSet (o nosso grupo de alunos)
        /// </summary>
        private async Task CriarFaceSetAsync()
        {
            // --- INÍCIO DA CORREÇÃO ---
            // Esta chamada também não envia ficheiros, por isso usa FormUrlEncodedContent
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("api_key", _apiKey),
                new KeyValuePair<string, string>("api_secret", _apiSecret),
                new KeyValuePair<string, string>("outer_id", _faceSetOuterId),
                new KeyValuePair<string, string>("display_name", "Alunos ETEC 3DSN 2025")
            });
            // --- FIM DA CORREÇÃO ---

            var response = await _httpClient.PostAsync($"{ApiBaseUrl}faceset/create", formData);

            if (!response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var errorJson = JObject.Parse(responseString);
                var errorMessage = errorJson["error_message"]?.ToString();

                Debug.WriteLine($"Falha ao criar o FaceSet: {errorMessage}");
                throw new Exception($"Falha ao criar o FaceSet: {errorMessage}");
            }
            Debug.WriteLine($"FaceSet '{_faceSetOuterId}' criado com sucesso.");
        }

        #endregion

        // --- NENHUMA ALTERAÇÃO NECESSÁRIA DAQUI PARA BAIXO ---
        // (Estas funções enviam imagens, por isso MultipartFormDataContent está CORRETO)

        #region 2. Funcionalidades Principais (Cadastro e Reconhecimento)

        /// <summary>
        /// Adiciona o rosto de um aluno ao nosso FaceSet (banco de dados).
        /// </summary>
        public async Task<bool> AdicionarRostoAoFaceSetAsync(Stream imagemStream, string alunoId)
        {
            try
            {
                // 1. Detetar o rosto na imagem para obter o "face_token"
                string faceToken = await DetectarRostoAsync(imagemStream);
                if (string.IsNullOrEmpty(faceToken))
                {
                    Debug.WriteLine("Nenhum rosto detetado na imagem.");
                    return false;
                }

                // 2. Adicionar o "face_token" ao nosso FaceSet
                var addFaceFormData = CreateBaseFormData();
                addFaceFormData.Add(new StringContent(_faceSetOuterId), "outer_id");
                addFaceFormData.Add(new StringContent(faceToken), "face_tokens");

                var addFaceResponse = await _httpClient.PostAsync($"{ApiBaseUrl}faceset/addface", addFaceFormData);
                if (!addFaceResponse.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Falha ao adicionar face ao FaceSet: {await addFaceResponse.Content.ReadAsStringAsync()}");
                    return false;
                }

                // 3. Associar o "face_token" ao ID do aluno
                var setUserIdFormData = CreateBaseFormData();
                setUserIdFormData.Add(new StringContent(faceToken), "face_token");
                setUserIdFormData.Add(new StringContent(alunoId), "user_id");

                var setUserIdResponse = await _httpClient.PostAsync($"{ApiBaseUrl}face/setuserid", setUserIdFormData);
                if (!setUserIdResponse.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Falha ao associar ID do aluno ao rosto: {await setUserIdResponse.Content.ReadAsStringAsync()}");
                    return false;
                }

                Debug.WriteLine($"Rosto para o aluno {alunoId} adicionado com sucesso.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro em AdicionarRostoAoFaceSetAsync: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Reconhece um rosto, comparando-o com o nosso banco de dados (FaceSet).
        /// </summary>
        public async Task<string> ReconhecerRostoAsync(Stream imagemStream)
        {
            try
            {
                var base64Image = await ConvertStreamToBase64(imagemStream);

                // 2. Pesquisar o rosto no nosso FaceSet (envia imagem, usa Multipart)
                var searchFormData = CreateBaseFormData();
                searchFormData.Add(new StringContent(base64Image), "image_base64");
                searchFormData.Add(new StringContent(_faceSetOuterId), "outer_id");

                var searchResponse = await _httpClient.PostAsync($"{ApiBaseUrl}search", searchFormData);

                if (!searchResponse.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Erro na pesquisa do Face++: {await searchResponse.Content.ReadAsStringAsync()}");
                    return null;
                }

                var responseString = await searchResponse.Content.ReadAsStringAsync();
                var searchJson = JObject.Parse(responseString);

                var results = searchJson["results"];
                if (results == null || !results.HasValues)
                {
                    Debug.WriteLine("Nenhuma correspondência encontrada.");
                    return null;
                }

                var bestMatch = results.OrderByDescending(r => (double)r["confidence"]).FirstOrDefault();
                if (bestMatch == null)
                {
                    Debug.WriteLine("Nenhuma correspondência encontrada.");
                    return null;
                }

                double confidence = (double)bestMatch["confidence"];
                string userId = bestMatch["user_id"]?.ToString();

                if (confidence > 80.0 && !string.IsNullOrEmpty(userId))
                {
                    Debug.WriteLine($"Rosto reconhecido: Aluno {userId} com confiança {confidence}");
                    return userId;
                }

                Debug.WriteLine($"Correspondência encontrada, mas abaixo do limiar de confiança ({confidence})");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro em ReconhecerRostoAsync: {ex.Message}");
                return null;
            }
        }


        #endregion

        #region Métodos Auxiliares

        /// <summary>
        /// Deteta um rosto numa imagem e retorna o "face_token" (ID do rosto).
        /// </summary>
        private async Task<string> DetectarRostoAsync(Stream imagemStream)
        {
            var base64Image = await ConvertStreamToBase64(imagemStream);

            // Esta chamada envia imagem, por isso CreateBaseFormData (Multipart) está CORRETO
            var formData = CreateBaseFormData();
            formData.Add(new StringContent(base64Image), "image_base64");

            var response = await _httpClient.PostAsync($"{ApiBaseUrl}detect", formData);

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"Erro ao detetar rosto: {await response.Content.ReadAsStringAsync()}");
                return null;
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseString);

            var faces = json["faces"];
            if (faces == null || !faces.HasValues)
            {
                Debug.WriteLine("Nenhum rosto detetado na imagem pelo /detect.");
                return null;
            }

            return faces[0]["face_token"]?.ToString();
        }

        /// <summary>
        /// Cria o formulário base (Multipart) com as chaves de API.
        /// (Este método é usado pelas funções que enviam imagens)
        /// </summary>
        private MultipartFormDataContent CreateBaseFormData()
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(_apiKey), "api_key");
            formData.Add(new StringContent(_apiSecret), "api_secret");
            return formData;
        }

        /// <summary>
        /// Converte um Stream para uma string Base64.
        /// </summary>
        private async Task<string> ConvertStreamToBase64(Stream stream)
        {
            if (stream is MemoryStream ms)
            {
                return Convert.ToBase64String(ms.ToArray());
            }

            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        #endregion
    }
}