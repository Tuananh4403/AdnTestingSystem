
using AdnTestingSystem.Core.UOW;
using Microsoft.Extensions.DependencyInjection;

namespace AdnTestingSystem.Core.Configuration
{
    public static class DIConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
            services.AddTransient(typeof(IUnitOfWork< , >), typeof(UnitOfWork< , >));
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

        }
    }
}
