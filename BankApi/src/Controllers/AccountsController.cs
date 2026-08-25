using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly Bank _bank;

    public AccountsController(Bank bank)
    {
        _bank = bank;
    }

    // GET /api/v1/accounts
    // GET /api/v1/accounts?minBalance=500
    // minBalance is optional — omit it to get every account, pass it to filter
    // down to accounts with a balance over that threshold.
    [HttpGet]
    public IActionResult GetAccounts([FromQuery] decimal? minBalance)
    {
        IEnumerable<Account> accounts = _bank.GetAllAccounts();

        if (minBalance.HasValue)
        {
            accounts = accounts.Where(a => a.Balance > minBalance.Value);
        }

        List<AccountSummary> result = accounts.Select(ToSummary).ToList();

        return Ok(result);
    }

    // GET /api/v1/accounts/customer/{username}
    // "Customer ID" here is the username, same convention as CustomersController.
    [HttpGet("customer/{username}")]
    public IActionResult GetAccountsByCustomer(string username)
    {
        User user = _bank.FindUser(username);

        if (user is not Customer customer)
        {
            return NotFound($"No customer found with username '{username}'.");
        }

        List<AccountSummary> result = customer.Accounts.Select(ToSummary).ToList();

        return Ok(result);
    }

    private static AccountSummary ToSummary(Account account)
    {
        return new AccountSummary
        {
            AccountNumber = account.AccountNumber,
            OwnerUsername = account.OwnerUsername,
            AccountType = account.GetAccountType(),
            Balance = account.Balance
        };
    }
}

public class AccountSummary
{
    public string AccountNumber { get; set; }
    public string OwnerUsername { get; set; }
    public string AccountType { get; set; }
    public decimal Balance { get; set; }
}