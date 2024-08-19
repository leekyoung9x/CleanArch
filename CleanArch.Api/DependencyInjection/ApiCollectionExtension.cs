using CleanArch.Api.Services;

namespace CleanArch.Api.DependencyInjection
{
    public static class ApiCollectionExtension
    {
        public static void RegisterServicesAPI(this IServiceCollection services)
        {
            services.AddTransient<IAuthService, AuthService>();
            services.AddHttpClient();
            services.AddScoped<IReCaptchaService, ReCaptchaService>();
            services.AddControllers();
        }
    }
}
