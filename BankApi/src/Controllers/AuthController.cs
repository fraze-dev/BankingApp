using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly Bank _bank;
    private readonly ICustomerRepository _customerRepository;

    public AuthController(Bank bank, ICustomerRepository customerRepository)
    {
        _bank = bank;
        _customerRepository = customerRepository;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Username and password are required.");
        }

        // Admins and the in-memory seeded customers live in Bank; customers
        // registered through POST /api/v1/customers only exist in MongoDB,
        // so fall back to checking there if Bank doesn't recognize them.
        User user = _bank.Authenticate(request.Username, request.Password);
        if (user != null)
        {
            return Ok(new LoginResponse { Username = user.Username, Role = user.GetRole() });
        }

        CustomerDocument customer = await _customerRepository.GetByUsernameAsync(request.Username);
        if (customer != null && PasswordUtil.Verify(customer.Password, request.Password))
        {
            return Ok(new LoginResponse { Username = customer.Username, Role = "Customer" });
        }

        return Unauthorized("Invalid username or password.");
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginResponse
{
    public string Username { get; set; }
    public string Role { get; set; }
}