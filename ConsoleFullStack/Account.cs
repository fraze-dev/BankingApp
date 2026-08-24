using System;

namespace BankingApp;

public abstract class Account
{
    private string accountNumber;
    private string ownerUsername;
    private decimal balance;

    // Overloaded constructors: default balance of 0, or an explicit starting balance
    protected Account(string accountNumber, string ownerUsername)
        : this(accountNumber, ownerUsername, 0m)
    {
    }

    protected Account(string accountNumber, string ownerUsername, decimal initialBalance)
    {
        this.accountNumber = accountNumber;
        this.ownerUsername = ownerUsername;
        this.balance = initialBalance;
    }

    // fields are private, exposed through properties
    public string AccountNumber => accountNumber;
    public string OwnerUsername => ownerUsername;
    public decimal Balance
    {
        get => balance;
        protected set => balance = value;
    }

    // Shared helper subclasses use to actually move the balance
    protected void AdjustBalance(decimal amount)
    {
        Balance += amount;
    }

    // Each subclass overrides this to report its own type
    public abstract string GetAccountType();

    public override string ToString()
    {
        return $"[{GetAccountType()}] {accountNumber} — Owner: {ownerUsername}, Balance: {balance:C}";
    }
}