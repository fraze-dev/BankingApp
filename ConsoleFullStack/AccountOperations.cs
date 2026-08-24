namespace BankingApp;

public interface AccountOperations
{
    void Deposit(decimal amount);
    bool Withdraw(decimal amount);
}