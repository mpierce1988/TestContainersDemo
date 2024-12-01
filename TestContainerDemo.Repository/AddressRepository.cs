using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TestContainerDemo.Model;
using TestContainerDemo.Service.Address;
using Exception = System.Exception;

namespace TestContainerDemo.Repository;

public class AddressRepository : IAddressRepository
{
    private readonly string _connectionString =
        "Server=database,1433;Database=AdventureWorks;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;";

    public AddressRepository(string connectionString = "")
    {
        if (!string.IsNullOrEmpty(connectionString))
        {
            _connectionString = connectionString;
        }
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    
    public async Task<IEnumerable<Address>> GetAllAsync()
    {
        try
        {
            using var connection = CreateConnection();
            
            var addresses = await connection.QueryAsync<Address>(
                "usp_GetAllAddresses",
                commandType: CommandType.StoredProcedure
            );

            return addresses;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Address?> GetByIdAsync(int id)
    {
        try
        {
            using var connection = CreateConnection();
            
            var parameters = new { AddressId = id };
            Address? address = await connection.QueryFirstOrDefaultAsync<Address>(
                "usp_GetAddressById",
                parameters,
                commandType: CommandType.StoredProcedure
                );

            return address;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<int?> AddAsync(Address address)
    {
        try
        {
            using var connection = CreateConnection();

            address.ModifiedDate = DateTime.Now;

            var parameters = new DynamicParameters();
            parameters.Add("@AddressLine1", address.AddressLine1, DbType.String);
            parameters.Add("@AddressLine2", address.AddressLine2, DbType.String);
            parameters.Add("@City", address.City, DbType.String);
            parameters.Add("@StateProvince", address.StateProvince, DbType.String);
            parameters.Add("@CountryRegion", address.CountryRegion, DbType.String);
            parameters.Add("@PostalCode", address.PostalCode, DbType.String);
            parameters.Add("@RowGuid", address.rowguid, DbType.Guid);
            parameters.Add("@ModifiedDate", address.ModifiedDate, DbType.DateTime);
            parameters.Add("@NewAddressID", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "usp_InsertAddress",
                parameters,
                commandType: CommandType.StoredProcedure
                );

            int newAddressId = parameters.Get<int>("@NewAddressID");
            return newAddressId;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Address address)
    {
        try
        {
            using var connection = CreateConnection();
            var parameters = new
            {
                AddressID = address.AddressID,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                City = address.City,
                StateProvince = address.StateProvince,
                CountryRegion = address.CountryRegion,
                PostalCode = address.PostalCode,
                ModifiedDate = address.ModifiedDate
            };

            int affectedRows = await connection.ExecuteAsync(
                "usp_UpdateAddress",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return affectedRows > 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            using var connection = CreateConnection();
            var parameters = new { AddressID = id };

            var affectedRows = await connection.ExecuteAsync(
                "usp_DeleteAddress",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return affectedRows > 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}