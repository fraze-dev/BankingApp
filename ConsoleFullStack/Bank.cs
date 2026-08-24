using System.Collections.Generic;

namespace BankingApp;

public class Bank
{
    private Dictionary<string, User> users;
    private Dictionary<string, Account> accounts;
    private int nextAccountId = 1006; // continues after seeded SAV-1001 / CHK-1001 / SAV-1002/CHK-1002/CHK-1003

    public Bank()
    {
        users = new Dictionary<string, User>();
        accounts = new Dictionary<string, Account>();
    }

    public void AddUser(User user)
    {
        users[user.Username] = user;
    }

    public void AddAccount(Account account)
    {
        accounts[account.AccountNumber] = account;

        if (users.TryGetValue(account.OwnerUsername, out User owner) && owner is Customer customer)
        {
            customer.AddAccount(account);
        }
    }

    public bool RemoveUser(string username)
    {
        return users.Remove(username);
    }

    public bool RemoveAccount(string accountNumber)
    {
        if (accounts.TryGetValue(accountNumber, out Account account))
        {
            if (users.TryGetValue(account.OwnerUsername, out User owner) && owner is Customer customer)
            {
                customer.Accounts.Remove(account);
            }
        }
        return accounts.Remove(accountNumber);
    }

    public string GenerateAccountNumber(string prefix)
    {
        return $"{prefix}-{nextAccountId++}";
    }

    public User FindUser(string username)
    {
        users.TryGetValue(username, out User user);
        return user;
    }

    public Account FindAccount(string accountNumber)
    {
        accounts.TryGetValue(accountNumber, out Account account);
        return account;
    }

    public List<User> GetAllUsers() => new List<User>(users.Values);

    public List<Account> GetAllAccounts() => new List<Account>(accounts.Values);

    public User Authenticate(string username, string password)
    {
        User user = FindUser(username);
        if (user != null && user.CheckPassword(password))
        {
            return user;
        }
        return null;
    }

    public void SeedData()
    {
        AddUser(new Admin("admin", "admin123"));

        AddUser(new Customer("jsmith", "pass123"));
        AddUser(new Customer("agarcia", "pass456"));

        AddAccount(new SavingsAccount("SAV-1001", "jsmith", 500m));
        AddAccount(new CheckingsAccount("CHK-1001", "jsmith", 200m, 10m));
        AddAccount(new CheckingsAccount("CHK-1002", "jsmith", 700m));
        AddAccount(new CheckingsAccount("CHK-1003", "jsmith"));
        AddAccount(new SavingsAccount("SAV-1002", "agarcia", 1500m));
    }
}