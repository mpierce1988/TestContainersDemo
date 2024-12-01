using TestContainerDemo.Repository;
using TestContainerDemo.Service.Address;

namespace TestContainerDemo.IntegrationTests;

[Collection("Database Collection")]
public class AddressServiceTests
{
    private readonly DatabaseFixture _fixture;
    private readonly IAddressService _addressService;
    
    public AddressServiceTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        var connectionString = fixture.GetConnectionString();
        var repository = new AddressRepository(connectionString);

        _addressService = new AddressService(repository);
    }

    [Fact]
    public async Task GetAddresses_ReturnsAddresses()
    {
        // Act
        var addresses = await _addressService.GetAllAsync();
        
        // Assert
        Assert.NotNull(addresses);
        Assert.NotEmpty(addresses);
    }
}