using System;

namespace BankApi;

public class SavingsAccount : Account, AccountOperations
{
    private decimal interestRate;

    public SavingsAccount(string accountNumber, string ownerUsername)
        : this(accountNumber, ownerUsername, 0m, 0.02m)
    {
    }

    public SavingsAccount(string accountNumber, string ownerUsername, decimal initialBalance)
        : this(accountNumber, ownerUsername, initialBalance, 0.02m)
    {
    }

    public SavingsAccount(string accountNumber, string ownerUsername, decimal initialBalance, decimal interestRate)
        : base(accountNumber, ownerUsername, initialBalance)
    {
        this.interestRate = interestRate;
    }

    public decimal InterestRate
    {
        get => interestRate;
        set => interestRate = value;
    }

    public override string GetAccountType() => "Savings";

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

        // No overdraft
        if (amount > Balance)
        {
            Console.WriteLine("Insufficient funds. Savings accounts do not allow overdraft.");
            return false;
        }

        AdjustBalance(-amount);
        return true;
    }
}