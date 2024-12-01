namespace TestContainerDemo.Service.Address;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _addressRepository;
    
    public AddressService(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }
    
    public async Task<IEnumerable<Model.Address>> GetAllAsync()
    {
        return await _addressRepository.GetAllAsync();
    }

    public async Task<Model.Address?> GetByIdAsync(int id)
    {
        return await _addressRepository.GetByIdAsync(id);
    }

    public async Task<Model.Address?> AddAsync(Model.Address address)
    {
        int? newId = await _addressRepository.AddAsync(address);
        
        if(newId is null) throw new Exception("Failed to add address");
        
        return await _addressRepository.GetByIdAsync(newId.Value);
    }

    public async Task<Model.Address?> UpdateAsync(Model.Address address)
    {
        bool updated = await _addressRepository.UpdateAsync(address);
        
        if(!updated) throw new Exception("Failed to update address");
        
        return await _addressRepository.GetByIdAsync(address.AddressID);
    }

    public async Task<Model.Address?> DeleteAsync(int id)
    {
        Model.Address? modelToDelete = await _addressRepository.GetByIdAsync(id);
        
        if(modelToDelete is null) throw new Exception("Address not found");
        
        bool deleted = await _addressRepository.DeleteAsync(id);
        
        if(!deleted) throw new Exception("Failed to delete address");
        
        return modelToDelete; 
    }
}