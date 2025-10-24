using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
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
        private readonly IFaceClient faceClient;
        private readonly string recognitionModel = RecognitionModel.Recognition04;

        public FaceRecognitionService(string endpoint, string key)
        {
            faceClient = new FaceClient(new ApiKeyServiceClientCredentials(key))
            {
                Endpoint = endpoint
            };
        }

        public async Task<User> ReconhecerAluno(Stream imagemCapturada, List<User> alunos)
        {
            var imagemDetectada = await faceClient.Face.DetectWithStreamAsync(
                imagemCapturada,
                returnFaceId: true,
                recognitionModel: recognitionModel);

            var faceCapturada = imagemDetectada.FirstOrDefault();
            if (faceCapturada == null) return null;

            foreach (var aluno in alunos)
            {
                var imagemAluno = Convert.FromBase64String(aluno.FaceImageBase64);
                using var streamAluno = new MemoryStream(imagemAluno);

                var faceAluno = await faceClient.Face.DetectWithStreamAsync(
                    streamAluno,
                    returnFaceId: true,
                    recognitionModel: recognitionModel);

                var faceIdAluno = faceAluno.FirstOrDefault()?.FaceId;
                if (faceIdAluno == null) continue;

                var verifyResult = await faceClient.Face.VerifyFaceToFaceAsync(faceCapturada.FaceId.Value, faceIdAluno.Value);

                if (verifyResult.IsIdentical && verifyResult.Confidence > 0.7)
                    return aluno;
            }

            return null;
        }
    }
}
