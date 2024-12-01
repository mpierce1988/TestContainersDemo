using TestContainerDemo.Repository;
using TestContainerDemo.Service.Address;

namespace TestContainerDemo.IntegrationTests;

[Collection("Database Collection")]
public class AddressServiceTests
{
    private readonly DatabaseFixture _fixture;
    private readonly IAddressService _addressService;

    private readonly VerifySettings _verifySettings;
    
    public AddressServiceTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        var connectionString = fixture.GetConnectionString();
        var repository = new AddressRepository(connectionString);

        _addressService = new AddressService(repository);

        _verifySettings = new VerifySettings();
        _verifySettings.UseDirectory("snapshots");
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

    [Fact]
    public async Task GetAddresses_SnapshotAllAddresses_ShouldReturnCorrectAddresses()
    {
        // Act
        var addresses = await _addressService.GetAllAsync();
        
        // Assert
        await Verify(addresses, _verifySettings);
    }
}