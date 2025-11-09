using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
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
        // Cliente do SDK Clássico
        private readonly IFaceClient faceClient;
        private string CurrentPersonGroupId => SecretsManager.PersonGroupId;

        public FaceRecognitionService()
        {
            // Inicialização do cliente clássico
            faceClient = new FaceClient(new ApiKeyServiceClientCredentials(SecretsManager.FaceApiKey))
            {
                Endpoint = SecretsManager.FaceApiEndpoint
            };
        }

        public async Task CriarGrupoSeNaoExistirAsync()
        {
            try
            {
                // Tenta pegar o grupo. Se não existir, ele lança uma exceção APIErrorException
                await faceClient.PersonGroup.GetAsync(CurrentPersonGroupId);
            }
            catch (APIErrorException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Grupo não encontrado (404), então criamos
                await faceClient.PersonGroup.CreateAsync(CurrentPersonGroupId, "Alunos ETEC SAAD", userData: "Criado pelo App SAAD");
                // Treina o grupo recém-criado (vazio) para evitar erros futuros
                await faceClient.PersonGroup.TrainAsync(CurrentPersonGroupId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar grupo: {ex.Message}");
                throw;
            }
        }

        public async Task<User> ReconhecerAluno(Stream imagemStream, List<User> listaDeAlunos)
        {
            try
            {
                // 1. Detectar rostos (SDK Clássico aceita Stream diretamente!)
                IList<DetectedFace> facesDetectadas = await faceClient.Face.DetectWithStreamAsync(
                    imagemStream,
                    returnFaceId: true,
                    recognitionModel: RecognitionModel.Recognition04,
                    detectionModel: DetectionModel.Detection03);

                if (facesDetectadas == null || facesDetectadas.Count == 0) return null;

                // 2. Identificar no grupo
                var faceIds = facesDetectadas.Select(f => f.FaceId.Value).ToList();

                var resultadosIdentificacao = await faceClient.Face.IdentifyAsync(faceIds, CurrentPersonGroupId);

                foreach (var resultado in resultadosIdentificacao)
                {
                    if (resultado.Candidates.Count > 0)
                    {
                        var melhorCandidato = resultado.Candidates.OrderByDescending(c => c.Confidence).First();

                        // Confiança mínima de 70%
                        if (melhorCandidato.Confidence > 0.70)
                        {
                            var aluno = listaDeAlunos.FirstOrDefault(a => a.AzurePersonId == melhorCandidato.PersonId.ToString());
                            if (aluno != null) return aluno;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no reconhecimento: {ex.Message}");
                return null;
            }
        }
    }
}