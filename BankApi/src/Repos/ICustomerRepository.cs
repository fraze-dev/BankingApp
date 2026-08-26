namespace BankApi;

public interface ICustomerRepository
{
    Task<List<CustomerDocument>> GetAllAsync();
    Task<CustomerDocument> GetByUsernameAsync(string username);
    Task CreateAsync(CustomerDocument customer);
    Task AddAccountNumberAsync(string username, string accountNumber);
}