using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using SAAD.Helpers;
using SAAD.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SAAD.Services
{
    public class FaceRecognitionService
    {
        private readonly IFaceClient faceClient;
        private string CurrentPersonGroupId => SecretsManager.PersonGroupId;

        public FaceRecognitionService()
        {
            faceClient = new FaceClient(new ApiKeyServiceClientCredentials(SecretsManager.FaceApiKey))
            {
                Endpoint = SecretsManager.FaceApiEndpoint
            };
        }

        // --- INICIALIZAÇÃO ---
        public async Task CriarGrupoSeNaoExistirAsync()
        {
            try
            {
                await faceClient.PersonGroup.GetAsync(CurrentPersonGroupId);
            }
            catch (APIErrorException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await faceClient.PersonGroup.CreateAsync(CurrentPersonGroupId, "Alunos ETEC SAAD");
                await TreinarGrupoAsync();
            }
        }

        // --- CADASTRO (ENROLLMENT) ---
        // 1. Cria a "pessoa" no Azure e retorna o ID dela
        public async Task<Guid> CriarPessoaNoAzureAsync(string nomeAluno)
        {
            var person = await faceClient.PersonGroupPerson.CreateAsync(CurrentPersonGroupId, nomeAluno);
            return person.PersonId;
        }

        // 2. Adiciona a foto do rosto a essa pessoa
        public async Task AdicionarRostoAsync(Guid personId, Stream imagemStream)
        {
            await faceClient.PersonGroupPerson.AddFaceFromStreamAsync(CurrentPersonGroupId, personId, imagemStream, detectionModel: DetectionModel.Detection03);
        }

        // 3. Treina o grupo (Obrigatório após adicionar qualquer pessoa ou rosto!)
        public async Task TreinarGrupoAsync()
        {
            await faceClient.PersonGroup.TrainAsync(CurrentPersonGroupId);

            // Espera o treinamento terminar
            while (true)
            {
                await Task.Delay(1000);
                var trainingStatus = await faceClient.PersonGroup.GetTrainingStatusAsync(CurrentPersonGroupId);
                if (trainingStatus.Status != TrainingStatusType.Running)
                {
                    break;
                }
            }
        }

        // --- RECONHECIMENTO ---
        public async Task<User> ReconhecerAluno(Stream imagemStream, List<User> listaDeAlunos)
        {
            try
            {
                IList<DetectedFace> facesDetectadas = await faceClient.Face.DetectWithStreamAsync(
                    imagemStream,
                    returnFaceId: true,
                    recognitionModel: RecognitionModel.Recognition04,
                    detectionModel: DetectionModel.Detection03);

                if (facesDetectadas == null || facesDetectadas.Count == 0) return null;

                var faceIds = facesDetectadas.Select(f => f.FaceId.Value).ToList();
                var resultados = await faceClient.Face.IdentifyAsync(faceIds, CurrentPersonGroupId);

                foreach (var resultado in resultados)
                {
                    if (resultado.Candidates.Count > 0)
                    {
                        var melhorCandidato = resultado.Candidates.OrderByDescending(c => c.Confidence).First();
                        if (melhorCandidato.Confidence > 0.70) // 70% de confiança
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