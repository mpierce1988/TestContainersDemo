using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestContainerDemo.Model;
using TestContainerDemo.Repository;
using TestContainerDemo.Service;
using TestContainerDemo.Service.Address;

namespace TestContainerDemo.Console;

class Program
{
    static async Task Main(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();
        
        IAddressRepository addressRepository = new AddressRepository();
        IAddressService addressService = new AddressService(addressRepository);
        AddressHelper addressHelper = new AddressHelper(addressService);

        await addressHelper.RunAddressProgram();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddRepository();
                services.AddService();
                services.AddLogging();

            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });
}