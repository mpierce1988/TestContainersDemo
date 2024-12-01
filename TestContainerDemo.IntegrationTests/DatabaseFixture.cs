using TestContainers.Container.Abstractions.Hosting;
using TestContainers.Container.Database.MsSql;
using Testcontainers.MsSql;
using MsSqlContainer = Testcontainers.MsSql.MsSqlContainer;

namespace TestContainerDemo.IntegrationTests;

public class DatabaseFixture : IAsyncLifetime
{
    public MsSqlContainer MsSqlContainer { get; private set; }
    
    public async Task InitializeAsync()
    {
        var builder = new MsSqlBuilder()
            .WithImage("sqlserver-prerestoredtwo")
            .WithCleanUp(true)
            .WithPortBinding(1433, 1433)
            .WithPassword("YourStrong!Passw0rd");

        MsSqlContainer = builder.Build();
        await MsSqlContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await MsSqlContainer.StopAsync();
    }

    public string GetConnectionString()
    {
        return
            $"Server=localhost,{MsSqlContainer.GetMappedPublicPort(1433)};Database=AdventureWorks;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;";
        return $"server=localhost,{MsSqlContainer.GetMappedPublicPort(1433)};database=AdventureWorks;user id=sa;password=YourStrong!Passw0rd;TrustServerCertificate=true;";
    }
}