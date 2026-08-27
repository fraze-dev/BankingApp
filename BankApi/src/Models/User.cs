namespace BankApi;

public abstract class User
{
    private string username;
    private string passwordHash;

    protected User(string username, string password)
    {
        this.username = username;
        this.passwordHash = PasswordUtil.Hash(password);
    }

    public string Username => username;

    public bool CheckPassword(string attemptedPassword)
    {
        return PasswordUtil.Verify(passwordHash, attemptedPassword);
    }

    // Updates the stored password, hashing the new value the same way the
    // constructor does — callers never see or persist a raw password.
    public void UpdatePassword(string newPassword)
    {
        passwordHash = PasswordUtil.Hash(newPassword);
    }

    public abstract string GetRole();

    public override string ToString()
    {
        return $"{GetRole()}: {username}";
    }
}