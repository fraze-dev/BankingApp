namespace BankingApp;

public class Admin : User
{
    public Admin(string username, string password)
        : base(username, password)
    {
    }

    public override string GetRole() => "Admin";
}