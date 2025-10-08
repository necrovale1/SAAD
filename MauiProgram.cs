using Microsoft.Extensions.Logging;
using SAAD2.Views;

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
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                    fonts.AddFont("Font Awesome 7 Free-Solid-900.otf", "FASolid");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // As linhas que registavam MateriaService e FaltaService foram removidas.
            return builder.Build();
        }
    }
}