using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICustomerRepository _customerRepository;

    public AccountsController(IAccountRepository accountRepository, ICustomerRepository customerRepository)
    {
        _accountRepository = accountRepository;
        _customerRepository = customerRepository;
    }

    // GET /api/v1/accounts
    // GET /api/v1/accounts?minBalance=500
    [HttpGet]
    public async Task<IActionResult> GetAccounts([FromQuery] decimal? minBalance)
    {
        List<AccountDocument> accounts = await _accountRepository.GetAllAsync(minBalance);
        return Ok(accounts.Select(ToSummary).ToList());
    }

    // GET /api/v1/accounts/customer/{username}
    [HttpGet("customer/{username}")]
    public async Task<IActionResult> GetAccountsByCustomer(string username)
    {
        CustomerDocument customer = await _customerRepository.GetByUsernameAsync(username);
        if (customer == null)
        {
            return NotFound($"No customer found with username '{username}'.");
        }

        List<AccountDocument> accounts = await _accountRepository.GetByCustomerUsernameAsync(username);
        return Ok(accounts.Select(ToSummary).ToList());
    }

    private static AccountSummary ToSummary(AccountDocument account)
    {
        return new AccountSummary
        {
            AccountNumber = account.AccountNumber,
            OwnerUsername = account.OwnerUsername,
            AccountType = account.AccountType,
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