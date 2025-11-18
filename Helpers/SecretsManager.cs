// Removida a linha "using SAAD.Helpers;" de cima
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
            var assembly = Assembly.GetExecutingAssembly();

            // --- INÍCIO DA CORREÇÃO DINÂMICA ---
            // Procura dinamicamente pelo recurso que termina com "secrets.json"
            string resourceName = assembly.GetManifestResourceNames()
                                          .FirstOrDefault(n => n.EndsWith("secrets.json", StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(resourceName))
            {
                System.Diagnostics.Debug.WriteLine($"ERRO CRÍTICO: Não foi encontrado NENHUM recurso terminando com 'secrets.json'.");
                _configuration = builder.Build(); // Constrói uma config vazia
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Info: Tentando carregar o recurso encontrado: '{resourceName}'");
            // --- FIM DA CORREÇÃO DINÂMICA ---

            using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream != null)
            {
                System.Diagnostics.Debug.WriteLine($"Sucesso: Recurso '{resourceName}' carregado.");
                builder.AddJsonStream(stream);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"ERRO CRÍTICO: Falha ao carregar o stream do recurso '{resourceName}', embora o nome tenha sido encontrado.");
            }

            _configuration = builder.Build();
        }

        // --- Propriedades (O seu código existente) ---
        public static string FirebaseUrl => _configuration["FirebaseDatabaseUrl"] ?? "";
        public static string FirebaseSecret => _configuration["FirebaseApiKey"] ?? "";

        public static string FaceApiKey => _configuration["FacePlusPlusApiKey"] ?? "";
        public static string FaceApiSecret => _configuration["FacePlusPlusApiSecret"] ?? "";

        public const string PersonGroupId = "alunos_etec_3dsn_2025";
    }

    public class SecretsMarker { }
}
// Removida a chaveta '}' extra que estava no ficheiro anterior