using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace SAAD2.Helpers
{
        public static class SecretsManager
        {
            private static IConfiguration Configuration => new ConfigurationBuilder()
                .AddUserSecrets<SecretsMarker>()
                .Build();

            public static string VisionApiKey => Configuration["VisionApiKey"];
            public static string FirebaseUrl => Configuration["FirebaseUrl"];
            public static string FirebaseSecret => Configuration["FirebaseSecret"];
        }

        public class SecretsMarker { }
    }
