using System;

namespace BankingApp;

public class CheckingsAccount : Account, AccountOperations
{
    private decimal overdraftLimit;

    public CheckingsAccount(string accountNumber, string ownerUsername)
        : this(accountNumber, ownerUsername, 0m, 50m)
    {
    }

    public CheckingsAccount(string accountNumber, string ownerUsername, decimal initialBalance)
        : this(accountNumber, ownerUsername, initialBalance, 100m)
    {
    }

    public CheckingsAccount(string accountNumber, string ownerUsername, decimal initialBalance, decimal overdraftLimit)
        : base(accountNumber, ownerUsername, initialBalance)
    {
        this.overdraftLimit = overdraftLimit;
    }

    public decimal OverdraftLimit
    {
        get => overdraftLimit;
        set => overdraftLimit = value;
    }

    public override string GetAccountType() => "Checking";

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Deposit amount must be positive.");
            return;
        }
        AdjustBalance(amount);
    }

    public bool Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Withdrawal amount must be positive.");
            return false;
        }

        // Checkings accounts allow overdraft up to a limit
        if (amount > Balance + overdraftLimit)
        {
            Console.WriteLine($"Insufficient funds. Overdraft limit is {overdraftLimit:C}.");
            return false;
        }

        AdjustBalance(-amount);
        return true;
    }
}