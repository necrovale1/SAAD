using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace SAAD.Helpers
{
    public static class SecretsManager
    {
        private static readonly IConfiguration _configuration;

        static SecretsManager()
        {
            var builder = new ConfigurationBuilder();

            // Pega o assembly (o executável) onde este código está rodando
            var assembly = Assembly.GetExecutingAssembly();

            // O nome do recurso é: NomeDoProjeto.NomeDoArquivo
            string resourceName = "SAAD.Helpers.secrets.json";

            using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream != null)
            {
                builder.AddJsonStream(stream);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"ERRO CRÍTICO: Não foi encontrado o recurso '{resourceName}'. Verifique se a Build Action está como Embedded Resource.");
            }

            _configuration = builder.Build();
        }

        // --- PROPRIEDADES ---
        // O operador '??' evita que o app crashe se a chave não for lida
        public static string FirebaseUrl => _configuration["FirebaseUrl"] ?? "";

        // Mapeamos a sua ApiKey para aqui para que o Login funcione
        public static string FirebaseSecret => _configuration["FirebaseSecret"] ?? "";

        public static string FaceApiKey => _configuration["FaceServiceKey"] ?? "";
        public static string FaceApiEndpoint => _configuration["FaceServiceEndpoint"] ?? "";

        public const string PersonGroupId = "alunos-etec-3dsn-2025";
    }

    public class SecretsMarker { }
}