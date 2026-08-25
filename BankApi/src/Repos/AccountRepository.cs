using MongoDB.Driver;

namespace BankApi;

public class AccountRepository : IAccountRepository
{
    private readonly IMongoCollection<AccountDocument> _accounts;

    public AccountRepository(IMongoDatabase database)
    {
        _accounts = database.GetCollection<AccountDocument>("accounts");
    }

    public async Task<List<AccountDocument>> GetAllAsync(decimal? minBalance)
    {
        FilterDefinition<AccountDocument> filter = minBalance.HasValue
            ? Builders<AccountDocument>.Filter.Gt(a => a.Balance, minBalance.Value)
            : FilterDefinition<AccountDocument>.Empty;

        return await _accounts.Find(filter).ToListAsync();
    }

    public async Task<List<AccountDocument>> GetByCustomerUsernameAsync(string username)
    {
        return await _accounts.Find(a => a.OwnerUsername == username).ToListAsync();
    }
}