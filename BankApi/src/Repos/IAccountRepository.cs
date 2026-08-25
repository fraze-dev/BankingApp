namespace BankApi;

public interface IAccountRepository
{
    Task<List<AccountDocument>> GetAllAsync(decimal? minBalance);
    Task<List<AccountDocument>> GetByCustomerUsernameAsync(string username);
}