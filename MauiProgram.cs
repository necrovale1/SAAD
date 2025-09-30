using SAAD2.Services;

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
                    fonts.AddFont("Font Awesome 7 Free-Solid-900.otf", "FontAwesomeSolid");
                    fonts.AddFont("Font Awesome 7 Free-Regular-400.otf", "FontAwesome");
                    fonts.AddFont("Font Awesome 7 Free-Brands-400.otf", "FontAwesomeBrands");
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