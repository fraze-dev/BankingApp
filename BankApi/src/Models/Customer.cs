using System.Collections.Generic;

namespace BankApi;

public class Customer : User
{
    private List<Account> accounts;

    // Overloaded constructors: start with no accounts, or hand in an existing list
    public Customer(string username, string password)
        : this(username, password, new List<Account>())
    {
    }

    public Customer(string username, string password, List<Account> accounts)
        : base(username, password)
    {
        this.accounts = accounts;
    }

    public List<Account> Accounts => accounts;

    public void AddAccount(Account account)
    {
        accounts.Add(account);
    }

    public override string GetRole() => "Customer";
}