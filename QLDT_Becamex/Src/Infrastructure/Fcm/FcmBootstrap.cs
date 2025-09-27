using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace QLDT_Becamex.Src.Infrastructure.Fcm
{
    public static class FcmBootstrap
    {
        public static IServiceCollection AddFcm(this IServiceCollection services, IConfiguration cfg)
        {
            FirebaseApp app;
            if (FirebaseApp.DefaultInstance == null)
            {
                var credentialsPath = cfg["Fcm:CredentialsPath"] ?? "secrets/firebase-service-account.json";
                app = FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(credentialsPath)
                });
            }
            else
            {
                app = FirebaseApp.DefaultInstance;
            }

            // Đăng ký vào DI
            services.AddSingleton(app);
            return services;
        }
    }
}
