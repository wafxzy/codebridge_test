using codebridge.BLL.Services;
using codebridge.BLL.Services.Interfaces;
using codebridge.DAL.Repositories;
using codebridge.DAL.Repositories.Interfaces;

namespace Codebridge.API.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServiceExtensions(this IServiceCollection Services)
        {
            Services.AddScoped<IDogRepository, DogRepository>();
            Services.AddScoped<IDogService, DogService>();


            return Services;

        }
    }
}
