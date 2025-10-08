#if ANDROID
using Android.Gms.Tasks;
using Android.Graphics;
using Google.MLKit.Vision.Common;
using Google.MLKit.Vision.Face;
using Java.Util; // Importante para a lista de rostos

#endif

namespace SAAD2.Services
{
    public class FaceDetectionService
    {
        public async Task<(bool hasFace, string message)> DetectFaceAsync(Stream photoStream)
        {
#if ANDROID
            try
            {
                var bitmap = await BitmapFactory.DecodeStreamAsync(photoStream);
                if (bitmap == null) return (false, "Não foi possível ler a imagem.");

                var image = InputImage.FromBitmap(bitmap, 0);
                var options = new FaceDetectorOptions.Builder()
                    .SetPerformanceMode(FaceDetectorOptions.PerformanceModeFast)
                    .Build();

                var detector = FaceDetection.GetClient(options);
                var result = await detector.Process(image).ToMauiTask();

                var faces = result as IList;

                // Para listas Java, usamos a propriedade .Size() em vez de .Count
                if (faces != null && faces.Size() > 0)
                {
                    if (faces.Size() == 1) return (true, "Rosto detetado com sucesso!");

                    return (false, $"Foram detetados {faces.Size()} rostos. Por favor, tire uma foto com apenas uma pessoa.");
                }

                return (false, "Nenhum rosto foi detetado na imagem. Tente novamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Erro técnico na deteção: {ex.Message}");
            }
#else
            return await Task.FromResult((false, "A deteção de rosto só está disponível para Android."));
#endif
        }
    }

#if ANDROID
    public static class TaskExtensions
    {
        public static Task<Java.Lang.Object> ToMauiTask(this Android.Gms.Tasks.Task task)
        {
            var tcs = new TaskCompletionSource<Java.Lang.Object>();
            task.AddOnSuccessListener(new OnSuccessListener(result => tcs.SetResult(result)));
            task.AddOnFailureListener(new OnFailureListener(ex => tcs.SetException(ex)));
            return tcs.Task;
        }

        private class OnSuccessListener : Java.Lang.Object, IOnSuccessListener
        {
            private readonly Action<Java.Lang.Object> _action;
            public OnSuccessListener(Action<Java.Lang.Object> action) => _action = action;
            public void OnSuccess(Java.Lang.Object result) => _action?.Invoke(result);
        }

        private class OnFailureListener : Java.Lang.Object, IOnFailureListener
        {
            private readonly Action<Java.Lang.Exception> _action;
            public OnFailureListener(Action<Java.Lang.Exception> action) => _action = action;
            public void OnFailure(Java.Lang.Exception e) => _action?.Invoke(e);
        }
    }
#endif
}