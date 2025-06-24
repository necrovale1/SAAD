using Microsoft.Extensions.Logging;

namespace SAAD
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

                    // Adicione as linhas abaixo para registrar as fontes do Material Symbols
                    // Assumindo que os nomes dos arquivos são estes. Ajuste se necessário.
                    fonts.AddFont("MaterialSymbolsOutlined-Regular.ttf", "MaterialOutlined");
                    fonts.AddFont("MaterialSymbolsRounded-Regular.ttf", "MaterialRounded");
                    fonts.AddFont("MaterialSymbolsSharp-Regular.ttf", "MaterialSharp");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}