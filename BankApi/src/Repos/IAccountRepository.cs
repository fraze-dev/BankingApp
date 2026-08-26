namespace BankApi;

public interface IAccountRepository
{
    Task<List<AccountDocument>> GetAllAsync(decimal? minBalance);
    Task<List<AccountDocument>> GetByCustomerUsernameAsync(string username);
    Task<AccountDocument> GetByAccountNumberAsync(string accountNumber);
    Task CreateAsync(AccountDocument account);
    Task UpdateBalanceAsync(string accountNumber, decimal newBalance);
    Task<string> GetNextAccountNumberAsync(string prefix);
}