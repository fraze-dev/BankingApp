using MongoDB.Driver;

namespace BankApi;

// Mirrors Bank.SeedData() from the no-DB branch, but writes into MongoDB
// instead of an in-memory Dictionary, and only runs once — if the customers
// collection already has data (e.g. on every app restart after the first
// run), it does nothing, so re-running the app doesn't keep duplicating data.
public static class MongoSeedData
{
    public static async Task SeedAsync(IMongoDatabase database)
    {
        var customers = database.GetCollection<CustomerDocument>("customers");
        var accounts = database.GetCollection<AccountDocument>("accounts");

        long existingCustomers = await customers.CountDocumentsAsync(FilterDefinition<CustomerDocument>.Empty);
        if (existingCustomers > 0)
        {
            return;
        }

        await customers.InsertManyAsync(new[]
        {
            new CustomerDocument
            {
                Username = "jsmith",
                Password = PasswordUtil.Hash("pass123"),
                AccountNumbers = new List<string> { "SAV-1001", "CHK-1001", "CHK-1002", "CHK-1003" }
            },
            new CustomerDocument
            {
                Username = "agarcia",
                Password = PasswordUtil.Hash("pass456"),
                AccountNumbers = new List<string> { "SAV-1002" }
            }
        });

        await accounts.InsertManyAsync(new[]
        {
            new AccountDocument { AccountNumber = "SAV-1001", OwnerUsername = "jsmith", AccountType = "Savings", Balance = 500m },
            new AccountDocument { AccountNumber = "CHK-1001", OwnerUsername = "jsmith", AccountType = "Checking", Balance = 200m },
            new AccountDocument { AccountNumber = "CHK-1002", OwnerUsername = "jsmith", AccountType = "Checking", Balance = 700m },
            new AccountDocument { AccountNumber = "CHK-1003", OwnerUsername = "jsmith", AccountType = "Checking", Balance = 0m },
            new AccountDocument { AccountNumber = "SAV-1002", OwnerUsername = "agarcia", AccountType = "Savings", Balance = 1500m }
        });
    }
}