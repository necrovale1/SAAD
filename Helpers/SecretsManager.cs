using Microsoft.Extensions.Configuration;


namespace SAAD.Helpers
{
    public static class SecretsManager
    {
        private static IConfiguration Configuration => new ConfigurationBuilder()
            .AddUserSecrets<SecretsMarker>()
            .Build();

        public static string VisionApiKey => Configuration["VisionApiKey"];
        public static string FirebaseUrl => Configuration["FirebaseUrl"];
        public static string FirebaseSecret => Configuration["FirebaseSecret"];
        public static string FaceServiceKey => Configuration["FaceServiceKey"];
        public static string FaceServiceEndpoint => Configuration["FaceServiceEndpoint"];
    }
}

    public class SecretsMarker { }

