using Microsoft.Extensions.DependencyInjection;
using TestContainerDemo.Service.Address;

namespace TestContainerDemo.Repository;

public static class Configuration
{
    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddScoped<IAddressRepository, AddressRepository>();
        return services;
    }
}