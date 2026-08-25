using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly Bank _bank;

    public CustomersController(Bank bank)
    {
        _bank = bank;
    }

    [HttpGet]
    public IActionResult GetCustomers()
    {
        List<CustomerSummary> customers = _bank.GetAllUsers()
            .OfType<Customer>()
            .Select(c => new CustomerSummary
            {
                Username = c.Username,
                AccountNumbers = c.Accounts.Select(a => a.AccountNumber).ToList()
            })
            .ToList();

        return Ok(customers);
    }
}

public class CustomerSummary
{
    public string Username { get; set; }
    public List<string> AccountNumbers { get; set; }
}