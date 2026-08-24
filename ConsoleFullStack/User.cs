namespace BankingApp;

public abstract class User
{
    private string username;
    private string password;

    protected User(string username, string password)
    {
        this.username = username;
        this.password = password;
    }

    public string Username => username;

    public bool CheckPassword(string attemptedPassword)
    {
        return password == attemptedPassword;
    }

    // update a password without
    public void UpdatePassword(string newPassword)
    {
        password = newPassword;
    }

    public abstract string GetRole();

    public override string ToString()
    {
        return $"{GetRole()}: {username}";
    }
}