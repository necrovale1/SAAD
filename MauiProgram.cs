using Microsoft.Extensions.Logging;
using SAAD2.Services;
using SAAD2.Views;
using SAAD2.Helpers;
using SAAD2.Models;

namespace SAAD2
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("materialdesignicons-webfont.ttf", "MaterialDesignIcons");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            // Register Services
            builder.Services.AddSingleton<MateriaService>();
            builder.Services.AddSingleton<FaltaService>();

            return builder.Build();
        }
    }
}