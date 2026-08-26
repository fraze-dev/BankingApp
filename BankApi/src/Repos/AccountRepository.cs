using MongoDB.Bson;
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

    public async Task<AccountDocument> GetByAccountNumberAsync(string accountNumber)
    {
        return await _accounts.Find(a => a.AccountNumber == accountNumber).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(AccountDocument account)
    {
        await _accounts.InsertOneAsync(account);
    }

    public async Task UpdateBalanceAsync(string accountNumber, decimal newBalance)
    {
        await _accounts.UpdateOneAsync(
            a => a.AccountNumber == accountNumber,
            Builders<AccountDocument>.Update.Set(a => a.Balance, newBalance));
    }

    public async Task<string> GetNextAccountNumberAsync(string prefix)
    {
        var filter = Builders<AccountDocument>.Filter.Regex(
            a => a.AccountNumber,
            new BsonRegularExpression($"^{prefix}-"));

        List<AccountDocument> matches = await _accounts.Find(filter).ToListAsync();

        int nextSuffix = 1001;
        if (matches.Count > 0)
        {
            int maxSuffix = matches
                .Select(a => int.TryParse(a.AccountNumber.Substring(prefix.Length + 1), out int value) ? value : 0)
                .Max();
            nextSuffix = maxSuffix + 1;
        }

        return $"{prefix}-{nextSuffix}";
    }
}
