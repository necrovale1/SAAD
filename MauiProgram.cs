using Camera.MAUI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SAAD.Services; 
using SAAD.Views;   

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
                    // Fontes e Emojis
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                    fonts.AddFont("Font Awesome 7 Free-Solid-900.otf", "FASolid");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            //  Carrega secrets.json
            builder.Configuration.AddUserSecrets<App>();

            // Serviços
            builder.Services.AddSingleton<FaceDetectionService>();
            builder.Services.AddSingleton<FaceRecognitionService>();

            // Páginas
            builder.Services.AddTransient<SplashPage>();
            builder.Services.AddTransient<CadastroAlunoPage>();
            builder.Services.AddTransient<CameraCapturePage>();
            builder.Services.AddTransient<CadastroAlunoPage>();
            builder.Services.AddTransient<ListagemFacialPage>();

            return builder.Build();
        }
    }
}