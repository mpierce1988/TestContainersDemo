namespace TestContainerDemo.Service.Address;

public interface IAddressRepository
{
    public Task<IEnumerable<Model.Address>> GetAllAsync();
    public Task<Model.Address?> GetByIdAsync(int id);
    public Task<int?> AddAsync(Model.Address address);
    public Task<bool> UpdateAsync(Model.Address address);
    public Task<bool> DeleteAsync(int id);
}