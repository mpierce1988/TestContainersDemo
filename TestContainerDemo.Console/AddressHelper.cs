using TestContainerDemo.Model;
using TestContainerDemo.Service.Address;

namespace TestContainerDemo.Console;

public class AddressHelper
{
    private readonly IAddressService _addressService;
    
    public AddressHelper(IAddressService addressService)
    {
        _addressService = addressService;
    }

    public async Task RunAddressProgram()
    {
        try
        {
            
            System.Console.WriteLine("*****Welcome to the Address Service Demo*****");

            bool repeat = true;

            while (repeat)
            {
                await GetShowAddresses();

                string insertQuestion = "Would you like to insert an address?";
                
                bool insertAddress = Utility.AskYesNoQuestion(insertQuestion);

                if (insertAddress)
                {
                    await AddRandomAddress();
                }
                
                string repeatQuestion = "Do you want to see all addresses again?";
                
                repeat = Utility.AskYesNoQuestion(repeatQuestion);
            }
            
            
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            throw;
        }
        
        System.Console.WriteLine("Press any key to exit...");
        System.Console.ReadLine();
    }

    private async Task GetShowAddresses()
    {
        System.Console.WriteLine("Getting all addresses...");

        List<Address>? addresses = await _addressService.GetAllAsync() as List<Model.Address>;
            
        System.Console.WriteLine("Finished getting all addresses");
        System.Console.WriteLine("Total addresses: " + addresses.Count());
    }

    private async Task AddRandomAddress()
    {
        Address newAddress = new Address()
        {
            AddressLine1 = "123 Main St",
            City = "Springfield",
            StateProvince = "IL",
            PostalCode = "62701",
            CountryRegion = "USA",
            rowguid = Guid.NewGuid()
        };
                    
        System.Console.WriteLine("Inserting address...");
                    
        Address? insertedAddress = await _addressService.AddAsync(newAddress);

        if (insertedAddress is null)
        {
            System.Console.WriteLine("Failed to insert address");
        }
        else
        {
            System.Console.WriteLine("Finished inserting address");
            System.Console.WriteLine("Inserted address ID: " + insertedAddress.AddressID);
        }
    }
}