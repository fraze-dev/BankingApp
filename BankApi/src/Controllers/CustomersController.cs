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

    // GET /api/v1/customers/{username}
    [HttpGet("{username}")]
    public IActionResult GetCustomerByUsername(string username)
    {
        User user = _bank.FindUser(username);

        if (user is not Customer customer)
        {
            return NotFound($"No customer found with username '{username}'.");
        }

        var summary = new CustomerSummary
        {
            Username = customer.Username,
            AccountNumbers = customer.Accounts.Select(a => a.AccountNumber).ToList()
        };

        return Ok(summary);
    }

    // POST /api/v1/customers
    [HttpPost]
    public IActionResult CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Username and password are required.");
        }

        if (_bank.FindUser(request.Username) != null)
        {
            return Conflict($"A user with username '{request.Username}' already exists.");
        }

        var customer = new Customer(request.Username, request.Password);
        _bank.AddUser(customer);

        var summary = new CustomerSummary
        {
            Username = customer.Username,
            AccountNumbers = new List<string>()
        };

        // 201 Created, with a Location header pointing at GET /api/v1/customers/{username}
        return CreatedAtAction(nameof(GetCustomerByUsername), new { username = customer.Username }, summary);
    }
}

public class CustomerSummary
{
    public string Username { get; set; }
    public List<string> AccountNumbers { get; set; }
}

public class CreateCustomerRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}