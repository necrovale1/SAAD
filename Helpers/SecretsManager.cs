using Microsoft.Extensions.Configuration;
using System.Reflection;
using SAAD.Helpers;

namespace SAAD.Helpers
{
    public static class SecretsManager
    {
        private static readonly IConfiguration _configuration;

        static SecretsManager()
        {
            var builder = new ConfigurationBuilder();
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = "SAAD.Helpers.secrets.json"; // O SDK trata disto agora

            using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream != null)
            {
                builder.AddJsonStream(stream);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"ERRO CRÍTICO: Não foi encontrado o recurso '{resourceName}'.");
            }

            _configuration = builder.Build();
        }

        // --- Propriedades do Firebase (Mantidas) ---
        public static string FirebaseUrl => _configuration["FirebaseDatabaseUrl"] ?? "";
        public static string FirebaseSecret => _configuration["FirebaseApiKey"] ?? "";

        // --- NOVAS Propriedades do Face++ ---
        public static string FaceApiKey => _configuration["FacePlusPlusApiKey"] ?? "";
        public static string FaceApiSecret => _configuration["FacePlusPlusApiSecret"] ?? "";

        // O "PersonGroup" do Azure é chamado de "outer_id" de um "FaceSet" no Face++
        // Vamos manter o nome da constante para não quebrar a SplashPage
        public const string PersonGroupId = "alunos_etec_3dsn_2025"; // Nota: Face++ não gosta de '-' no ID. Mudei para '_'.
    }

    public class SecretsMarker { }
}