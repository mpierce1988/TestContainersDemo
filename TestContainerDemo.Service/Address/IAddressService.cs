namespace TestContainerDemo.Service.Address;

public interface IAddressService
{
    public Task<IEnumerable<Model.Address>> GetAllAsync();
    public Task<Model.Address?> GetByIdAsync(int id);
    public Task<Model.Address?> AddAsync(Model.Address address);
    public Task<Model.Address?> UpdateAsync(Model.Address address);
    public Task<Model.Address?> DeleteAsync(int id);
}

public record GetAddressRequest(int Id);

