using Camera.MAUI;
using Microsoft.Extensions.Logging;
using SAAD.Services; // 👈 Adicione este 'using'
using SAAD.Views;   // 👈 Adicione este 'using'

namespace SAAD
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCameraView()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                    fonts.AddFont("Font Awesome 7 Free-Solid-900.otf", "FASolid");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // =========================================================
            // ▼▼ GARANTA QUE ESTAS LINHAS ESTÃO AQUI ▼▼
            // =========================================================

            // Registra os serviços para que o app saiba como criá-los
            builder.Services.AddSingleton<FaceDetectionService>();
            builder.Services.AddSingleton<FaceRecognitionService>();

            // Registra as páginas que vão RECEBER os serviços
            // (Transient = cria uma nova página toda vez que é chamada)
            builder.Services.AddTransient<SplashPage>();
            // Adicione suas outras páginas que usam serviços aqui...
            // ex: builder.Services.AddTransient<CadastroFacialPage>();

            // =========================================================

            return builder.Build();
        }
    }
}