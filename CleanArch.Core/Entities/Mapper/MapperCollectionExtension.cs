using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Core.Entities.Mapper
{
    public static class ServiceCollectionExtension
    {
        public static void RegisterMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(CreateProfileMapper));
        }
    }
}