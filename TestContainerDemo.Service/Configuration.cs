using Microsoft.Extensions.DependencyInjection;
using TestContainerDemo.Service.Address;

namespace TestContainerDemo.Service;

public static class Configuration
{
    public static IServiceCollection AddService(this IServiceCollection services)
    {
        services.AddScoped<IAddressService, AddressService>();
        return services;
    }
}