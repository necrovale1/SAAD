using Microsoft.Extensions.Configuration;

namespace SAAD.Helpers
{
    public static class SecretsManager
    {
        private static readonly IConfiguration _configuration;

        static SecretsManager()
        {
            _configuration = new ConfigurationBuilder()
                .AddUserSecrets<SecretsMarker>()
                .Build();
        }

        // --- FIREBASE ---
        public static string FirebaseUrl => _configuration["FirebaseUrl"];
        public static string FirebaseSecret => _configuration["FirebaseSecret"];

        // --- AZURE FACE API ---
        // Use estes nomes exatos no seu secrets.json
        public static string FaceApiKey => _configuration["FaceServiceKey"];
        public static string FaceApiEndpoint => _configuration["FaceServiceEndpoint"];

        // --- CONSTANTES ---
        public const string PersonGroupId = "alunos-etec-3dsn-2025";
    }

    public class SecretsMarker { }
}