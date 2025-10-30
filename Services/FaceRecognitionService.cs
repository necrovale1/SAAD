using Azure;
using Azure.AI.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using SAAD.Helpers;
using SAAD.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SAAD.Services
{
    public class FaceRecognitionService
    {
        private readonly FaceClient faceClient;

        // --- DEFINA O ID DO SEU GRUPO AQUI ---
        // Este é o ID (string) que você criou com o App de Console.
        // Ex: "alunos-etec-3dsn-2025"
        private const string SEU_GRUPO_ID = "alunos-etec-3dsn-2025";

        public FaceRecognitionService()
        {
            // Pega as credenciais do seu SecretsManager
            faceClient = new FaceClient(
                new Uri(SecretsManager.AzureEndpoint),
                new AzureKeyCredential(SecretsManager.AzureKey));
        }

        // (Este construtor alternativo está no seu GitHub, pode manter se precisar)
        public FaceRecognitionService(string endpoint, string key)
        {
            faceClient = new FaceClient(new Uri(endpoint), new AzureKeyCredential(key));
        }

        /// <summary>
        /// Reconhece um rosto em um stream comparando-o com um LargePersonGroup no Azure.
        /// </summary>
        /// <param name="imagemStream">Stream da foto tirada pela câmera</param>
        /// <param name="listaDeAlunos">Lista de todos os alunos vinda do Firebase</param>
        /// <returns>O objeto 'User' do aluno reconhecido, ou 'null' se não for encontrado.</returns>
        public async Task<User> ReconhecerAluno(Stream imagemStream, List<User> listaDeAlunos)
        {
            try
            {
                // 1. Detectar o(s) rosto(s) na imagem capturada
                // Usamos Recognition04 pois é o mais moderno
                var detectResponse = await faceClient.DetectAsync(
                    imagemStream,
                    FaceDetectionModel.Detection03,
                    FaceRecognitionModel.Recognition04,
                    returnFaceId: true); // Precisamos do FaceId para identificar

                IReadOnlyList<FaceDetectionResult> facesDetectadas = detectResponse.Value;

                if (facesDetectadas.Count == 0)
                {
                    // Nenhum rosto foi detectado pela API
                    return null;
                }

                // Pega os IDs (Guid) de todos os rostos detectados na foto
                var faceIds = facesDetectadas.Select(f => f.FaceId.Value).ToList();

                // 2. Identificar os rostos no seu grupo de alunos
                var identifyResponse = await faceClient.IdentifyFromLargePersonGroupAsync(
                    faceIds,
                    SEU_GRUPO_ID);

                IReadOnlyList<FaceIdentificationResult> resultados = identifyResponse.Value;

                foreach (var resultado in resultados)
                {
                    // Verifica se há candidatos (correspondências) para este rosto
                    if (resultado.Candidates.Count > 0)
                    {
                        // Pega o candidato com maior confiança
                        var candidato = resultado.Candidates.First();
                        Guid personIdDoAzure = candidato.PersonId;
                        double confianca = candidato.Confidence;

                        // 3. Verificar a confiança
                        // (Ajuste este valor conforme seus testes. 0.75 é um bom começo)
                        if (confianca > 0.70)
                        {
                            // 4. Cruzar o 'PersonId' do Azure com sua lista do Firebase
                            var alunoEncontrado = listaDeAlunos.FirstOrDefault(a =>
                                !string.IsNullOrEmpty(a.AzurePersonId) &&
                                a.AzurePersonId == personIdDoAzure.ToString());

                            if (alunoEncontrado != null)
                            {
                                // Sucesso! Retorna o aluno
                                return alunoEncontrado;
                            }
                        }
                    }
                }

                // Se chegou aqui, um rosto foi detectado, mas não 
                // correspondeu a ninguém do grupo com confiança suficiente.
                return null;
            }
            catch (RequestFailedException ex)
            {
                // Erro comum: o grupo não foi treinado ou o ID está errado
                Console.WriteLine($"Erro na API do Azure: {ex.Message}");
                // (Opcional: Exibir um DisplayAlert para o usuário)
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                return null;
            }
        }

        // O método 'CompararAsync' (Verificação 1:1) que estava aqui antes
        // não é mais usado por esta página.
    }
}